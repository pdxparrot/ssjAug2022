using Godot;

using System;

using pdxpartyparrot.ssjAug2022.Managers;
using pdxpartyparrot.ssjAug2022.Util;

namespace pdxpartyparrot.ssjAug2022.NPCs.AI
{
    public abstract class AgentSteering<T> : Node where T : SimpleNPC
    {
        private const float DecelerationTweaker = 0.3f;

        public struct SeekParams
        {
            public Vector3 target;

            public float maxSpeed;
        }

        public enum ArriveDeceleration
        {
            Fast = 1,

            Normal = 2,

            Slow = 3,
        }

        public struct ArriveParams
        {
            public Vector3 target;

            public float maxSpeed;

            public ArriveDeceleration deceleration;
        }

        public struct PursuitParams
        {
            public SimpleCharacter target;

            public float maxSpeed;

            internal ulong lastTargetUpdate;
        }

        public struct WanderParams
        {
            public float radius;

            public float distance;

            public float jitter;

            public float maxSpeed;

            internal Vector3 target;
        }

        [Flags]
        private enum SteeringBehavior
        {
            None = 0,

            Seek = 1,

            Arrive = 2,

            Pursuit = 4,

            Wander = 8,
        }

        private T _owner;

        private SteeringBehavior _enabledBehaviors = SteeringBehavior.None;

        private SeekParams _seekParams;

        private ArriveParams _arriveParams;

        private PursuitParams _pursuitParams;

        private WanderParams _wanderParams;

        #region Godot Lifecycle

        public override void _Ready()
        {
            _owner = GetOwner<T>();
        }

        #endregion

        #region Enable / Disable

        public void SeekOn(SeekParams seekParams)
        {
            _seekParams = seekParams;

            _owner.SetTarget(_seekParams.target);

            _enabledBehaviors |= SteeringBehavior.Seek;
        }

        public void SeekOff()
        {
            _enabledBehaviors &= ~SteeringBehavior.Seek;
        }

        public void ArriveOn(ArriveParams arriveParams)
        {
            _arriveParams = arriveParams;

            _owner.SetTarget(_arriveParams.target);

            _enabledBehaviors |= SteeringBehavior.Arrive;
        }

        public void ArriveOff()
        {
            _enabledBehaviors &= ~SteeringBehavior.Arrive;
        }

        public void PursuitOn(PursuitParams pursuitParams)
        {
            _pursuitParams = pursuitParams;

            _owner.SetTarget(_pursuitParams.target.GlobalTranslation);
            _pursuitParams.lastTargetUpdate = Time.GetTicksMsec();

            _enabledBehaviors |= SteeringBehavior.Pursuit;
        }

        public void PursuitOff()
        {
            _enabledBehaviors &= ~SteeringBehavior.Pursuit;
        }

        public void WanderOn(WanderParams wanderParams)
        {
            _wanderParams = wanderParams;

            // start with a random point on the circle
            double theta = PartyParrotManager.Instance.Random.NextSingle() * 2.0 * Math.PI;
            _wanderParams.target = new Vector3((float)Math.Cos(theta), 0.0f, (float)Math.Sin(theta)) * _wanderParams.radius;

            // 0 jitter to get the world target we just calculated
            var target = GetWanderTarget(0.0f);
            _owner.SetTarget(target);

            _enabledBehaviors |= SteeringBehavior.Wander;
        }

        public void WanderOff()
        {
            _enabledBehaviors &= ~SteeringBehavior.Wander;
        }

        private bool On(SteeringBehavior query)
        {
            return (_enabledBehaviors & query) == query;
        }

        #endregion

        public void Update(float delta)
        {
            if(On(SteeringBehavior.Seek)) {
                Seek();
            }

            if(On(SteeringBehavior.Arrive)) {
                Arrive();
            }

            if(On(SteeringBehavior.Pursuit)) {
                Pursuit();
            }

            if(On(SteeringBehavior.Wander)) {
                Wander(delta);
            }
        }

        #region Steering Behaviors

        // TODO: IsTargetReachable() will return false even if there is a reachable
        // final location so we probably want to do something else to prevent
        // getting stuck trying to reach something unreachable
        // (go as far as we can down the path and then do something else)

        private void Seek()
        {
            if(!_owner.IsTargetReachable() || _owner.IsTargetReached()) {
                _owner.SetVelocity(Vector3.Zero);
                return;
            }

            Seek(_owner.GetNextLocation(), _seekParams.maxSpeed);
        }

        private void Seek(Vector3 target, float maxSpeed)
        {
            var desiredVelocity = (target - _owner.GlobalTranslation).Normalized() * maxSpeed;
            _owner.SetVelocity(desiredVelocity - _owner.Velocity);
        }

        private void Arrive()
        {
            if(!_owner.IsTargetReachable() || _owner.IsTargetReached()) {
                _owner.SetVelocity(Vector3.Zero);
                return;
            }

            var target = _owner.GetNextLocation();
            var toTarget = target - _owner.GlobalTranslation;
            float distance = toTarget.Length();

            float speed = distance / ((int)_arriveParams.deceleration * DecelerationTweaker);
            speed = Math.Min(speed, _arriveParams.maxSpeed);

            var desiredVelocity = toTarget * speed / distance;
            _owner.SetVelocity(desiredVelocity - _owner.Velocity);
        }

        private void Pursuit()
        {
            if((!_owner.IsTargetReachable() || _owner.IsTargetReached()) && _owner.IsNavigationFinished()) {
                _owner.SetTarget(_pursuitParams.target.GlobalTranslation);
                _pursuitParams.lastTargetUpdate = Time.GetTicksMsec();
                return;
            }

            // spend a second seeking the last target
            if(Time.GetTicksMsec() - _pursuitParams.lastTargetUpdate < 1000) {
                Seek(_owner.GetNextLocation(), _seekParams.maxSpeed);
                return;
            }

            var target = _owner.GetNextLocation();
            var toEvader = target - _owner.GlobalTranslation;

            // if the evader is ahead and facing us, we can just seek it
            // acos(0.95) = 18 degrees
            float relativeHeading = _owner.Heading.Dot(_pursuitParams.target.Heading);
            if(toEvader.Dot(_owner.Heading) > 0.0f && relativeHeading < -0.95f) {
                Seek(target, _pursuitParams.maxSpeed);
                return;
            }

            float lookAheadTime = toEvader.Length() / (_pursuitParams.maxSpeed + _pursuitParams.target.MaxSpeed);
            Seek(target + _pursuitParams.target.Velocity * lookAheadTime, _pursuitParams.maxSpeed);
        }

        private Vector3 GetWanderTarget(float jitter)
        {
            // offset slightly on the circle
            _wanderParams.target += new Vector3(
                PartyParrotManager.Instance.Random.NextSingle(-1.0f, 1.0f) * jitter,
                0.0f,
                PartyParrotManager.Instance.Random.NextSingle(-1.0f, 1.0f) * jitter
            );

            // put the jitter target back on the circle
            var wanderTarget = _wanderParams.target.Normalized();
            _wanderParams.target = wanderTarget * _wanderParams.radius;

            // project the circle in the heading direction
            return _owner.GlobalTranslation + _wanderParams.target + (_owner.Heading * _wanderParams.distance);
        }

        private void Wander(float delta)
        {
            // seek the target until we reach it
            if(_owner.IsTargetReachable() && !_owner.IsTargetReached()) {
                Seek(_owner.GetNextLocation(), _wanderParams.maxSpeed);
                return;
            }

            /*if(!_owner.IsNavigationFinished()) {
                return;
            }*/

            // update the target
            float jitter = _wanderParams.jitter * delta;
            var target = GetWanderTarget(jitter);
            _owner.SetTarget(target);
        }

        #endregion
    }
}

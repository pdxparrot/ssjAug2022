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

        // TODO: this should use weighted prioritization for accumulation
        public void Update(float delta)
        {
            bool velocityUpdated = false;
            bool shouldStop = false;
            Vector3 velocity = Vector3.Zero;

            if(On(SteeringBehavior.Seek)) {
                (bool updated, bool stop) = Seek(out Vector3 seekVelocity);
                shouldStop |= stop;
                if(updated) {
                    velocity += seekVelocity;
                    velocityUpdated = true;
                }
            }

            if(On(SteeringBehavior.Arrive)) {
                (bool updated, bool stop) = Arrive(out Vector3 arriveVelocity);
                shouldStop |= stop;
                if(updated) {
                    velocity += arriveVelocity;
                    velocityUpdated = true;
                }
            }

            if(On(SteeringBehavior.Pursuit)) {
                if(Pursuit(out Vector3 pursuitVelocity)) {
                    velocity += pursuitVelocity;
                    velocityUpdated = true;
                }
            }

            if(On(SteeringBehavior.Wander)) {
                (bool updated, bool stop) = Wander(delta, out Vector3 wanderVelocity);
                shouldStop |= stop;
                if(updated) {
                    velocity += wanderVelocity;
                    velocityUpdated = true;
                }
            }

            if(shouldStop) {
                _owner.Stop();
            } else if(velocityUpdated) {
                _owner.SetVelocity(velocity);
            }
        }

        #region Steering Behaviors

        // TODO: IsTargetReachable() will return false even if there is a reachable
        // final location so we probably want to do something else to prevent
        // getting stuck trying to reach something unreachable
        // (go as far as we can down the path and then do something else)

        private (bool, bool) Seek(out Vector3 velocity)
        {
            velocity = Vector3.Zero;

            if(!_owner.IsTargetReachable() || _owner.IsTargetReached() /*|| !_owner.IsNavigationFinished()*/) {
                return (false, true);
            }

            velocity = Seek(_owner.GetNextLocation(), _seekParams.maxSpeed);
            return (true, false);
        }

        private Vector3 Seek(Vector3 target, float maxSpeed)
        {
            return (target - _owner.GlobalTranslation).Normalized() * maxSpeed;
        }

        private (bool, bool) Arrive(out Vector3 velocity)
        {
            velocity = Vector3.Zero;

            if(!_owner.IsTargetReachable() || _owner.IsTargetReached() /*|| !_owner.IsNavigationFinished()*/) {
                return (false, true);
            }

            var target = _owner.GetNextLocation();
            var toTarget = target - _owner.GlobalTranslation;
            float distance = toTarget.Length();

            float speed = distance / ((int)_arriveParams.deceleration * DecelerationTweaker);
            speed = Math.Min(speed, _arriveParams.maxSpeed);

            velocity = toTarget * speed / distance;
            return (true, false);
        }

        private bool Pursuit(out Vector3 velocity)
        {
            velocity = Vector3.Zero;

            if(!_owner.IsTargetReachable() || _owner.IsTargetReached() || Time.GetTicksMsec() - _pursuitParams.lastTargetUpdate > 500) {
                _owner.SetTarget(_pursuitParams.target.GlobalTranslation);
                _pursuitParams.lastTargetUpdate = Time.GetTicksMsec();
                return false;
            }

            var target = _owner.GetNextLocation();
            var toTarget = target - _owner.GlobalTranslation;

            float lookAheadTime = toTarget.Length() / (_pursuitParams.maxSpeed + _pursuitParams.target.HorizontalSpeed);
            velocity = Seek(target + _pursuitParams.target.Velocity * lookAheadTime, _pursuitParams.maxSpeed);
            return true;
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

        private (bool, bool) Wander(float delta, out Vector3 velocity)
        {
            velocity = Vector3.Zero;

            /*if(!_owner.IsNavigationFinished()) {
                return (false, true);
            }*/

            // seek the target until we reach it
            if(_owner.IsTargetReachable() && !_owner.IsTargetReached()) {
                velocity = Seek(_owner.GetNextLocation(), _wanderParams.maxSpeed);
                return (true, false);
            }

            // update the target
            float jitter = _wanderParams.jitter * delta;
            var target = GetWanderTarget(jitter);
            _owner.SetTarget(target);

            return (false, false);
        }

        #endregion
    }
}

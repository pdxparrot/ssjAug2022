using Godot;

using System;

using pdxpartyparrot.ssjAug2022.Managers;
using pdxpartyparrot.ssjAug2022.Util;

namespace pdxpartyparrot.ssjAug2022.NPCs.AI
{
    public abstract class Steering<T> : Node where T : SimpleNPC
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

        public struct FleeParams
        {
            public Vector3 target;

            public float maxSpeed;
        }

        public struct PursuitParams
        {
            public SimpleCharacter target;

            public float maxSpeed;
        }

        public struct EvadeParams
        {
            public SimpleCharacter pursuer;

            public float maxSpeed;
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

            Flee = 4,

            Pursuit = 8,

            Evade = 16,

            Wander = 32,
        }

        private T _owner;

        private SteeringBehavior _enabledBehaviors = SteeringBehavior.None;

        private SeekParams _seekParams;

        private ArriveParams _arriveParams;

        private FleeParams _fleeParams;

        private PursuitParams _pursuitParams;

        private EvadeParams _evadeParams;

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

            _enabledBehaviors |= SteeringBehavior.Seek;
        }

        public void SeekOff()
        {
            _enabledBehaviors &= ~SteeringBehavior.Seek;
        }

        public void ArriveOn(ArriveParams arriveParams)
        {
            _arriveParams = arriveParams;

            _enabledBehaviors |= SteeringBehavior.Arrive;
        }

        public void ArriveOff()
        {
            _enabledBehaviors &= ~SteeringBehavior.Arrive;
        }

        public void FleeOn(FleeParams fleeParams)
        {
            _fleeParams = fleeParams;

            _enabledBehaviors |= SteeringBehavior.Flee;
        }

        public void FleeOff()
        {
            _enabledBehaviors &= ~SteeringBehavior.Flee;
        }

        public void PursuitOn(PursuitParams pursuitParams)
        {
            _pursuitParams = pursuitParams;

            _enabledBehaviors |= SteeringBehavior.Pursuit;
        }

        public void PursuitOff()
        {
            _enabledBehaviors &= ~SteeringBehavior.Pursuit;
        }

        public void EvadeOn(EvadeParams evadeParams)
        {
            _evadeParams = evadeParams;

            _enabledBehaviors |= SteeringBehavior.Evade;
        }

        public void EvadeOff()
        {
            _enabledBehaviors &= ~SteeringBehavior.Evade;
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

        // call each _PhysicsProcess and ApplyForce()
        // can be useful to multiply by the owners mass as well
        public Vector3 Calculate(float delta)
        {
            var steeringForce = Vector3.Zero;

            if(On(SteeringBehavior.Seek)) {
                steeringForce += Seek();
            }

            if(On(SteeringBehavior.Arrive)) {
                steeringForce += Arrive();
            }

            if(On(SteeringBehavior.Flee)) {
                steeringForce += Flee();
            }

            if(On(SteeringBehavior.Pursuit)) {
                steeringForce += Pursuit();
            }

            if(On(SteeringBehavior.Evade)) {
                steeringForce += Evade();
            }

            if(On(SteeringBehavior.Wander)) {
                steeringForce += Wander(delta);
            }

            return steeringForce;
        }

        #region Steering Behaviors

        private Vector3 Seek()
        {
            return Seek(_seekParams.target, _seekParams.maxSpeed);
        }

        private Vector3 Seek(Vector3 target, float maxSpeed)
        {
            var desiredVelocity = (target - _owner.GlobalTranslation).Normalized() * maxSpeed;
            return desiredVelocity - _owner.Velocity;
        }

        private Vector3 Arrive()
        {
            var toTarget = _arriveParams.target - _owner.GlobalTranslation;

            float distance = toTarget.Length();
            if(distance <= 0.0f) {
                return Vector3.Zero;
            }

            float speed = distance / ((int)_arriveParams.deceleration * DecelerationTweaker);
            speed = Math.Min(speed, _arriveParams.maxSpeed);

            var desiredVelocity = toTarget * speed / distance;
            return desiredVelocity - _owner.Velocity;
        }

        private Vector3 Flee()
        {
            return Flee(_fleeParams.target, _fleeParams.maxSpeed);
        }

        private Vector3 Flee(Vector3 target, float maxSpeed)
        {
            var desiredVelocity = (_owner.GlobalTranslation - target).Normalized() * maxSpeed;
            return desiredVelocity - _owner.Velocity;
        }

        private Vector3 Pursuit()
        {
            var toEvader = _pursuitParams.target.GlobalTranslation - _owner.GlobalTranslation;

            // if the evader is ahead and facing us, we can just seek it
            // acos(0.95) = 18 degrees
            float relativeHeading = _owner.Heading.Dot(_pursuitParams.target.Heading);
            if(toEvader.Dot(_owner.Heading) > 0.0f && relativeHeading < -0.95f) {
                return Seek(_pursuitParams.target.GlobalTranslation, _pursuitParams.maxSpeed);
            }

            float lookAheadTime = toEvader.Length() / (_pursuitParams.maxSpeed + _pursuitParams.target.HorizontalSpeed);
            return Seek(_pursuitParams.target.GlobalTranslation + _pursuitParams.target.Velocity * lookAheadTime, _pursuitParams.maxSpeed);
        }

        private Vector3 Evade()
        {
            var toPursuer = _evadeParams.pursuer.GlobalTranslation - _owner.GlobalTranslation;

            float lookAheadTime = toPursuer.Length() / (_evadeParams.maxSpeed + _evadeParams.pursuer.HorizontalSpeed);
            return Flee(_evadeParams.pursuer.GlobalTranslation + _evadeParams.pursuer.Velocity * lookAheadTime, _evadeParams.maxSpeed);
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

        private Vector3 Wander(float delta)
        {
            float jitter = _wanderParams.jitter * delta;
            var target = GetWanderTarget(jitter);
            return Seek(target, _wanderParams.maxSpeed);
        }

        // TODO: path follow

        #endregion
    }
}

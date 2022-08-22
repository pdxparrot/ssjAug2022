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

        public struct PursuitParams
        {
            public SimpleCharacter target;

            public float maxSpeed;
        }

        public struct WanderParams
        {
            public float radius;

            public float distance;

            public float jitter;

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

        public void PursuitOn(PursuitParams pursuitParams)
        {
            _pursuitParams = pursuitParams;

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
            _wanderParams.target = new Vector3(_wanderParams.radius * (float)Math.Cos(theta), 0.0f, _wanderParams.radius * (float)Math.Sin(theta));

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

        public Vector3 Calculate(float delta)
        {
            var steeringForce = Vector3.Zero;

            if(On(SteeringBehavior.Seek)) {
                steeringForce += Seek();
            }

            if(On(SteeringBehavior.Arrive)) {
                steeringForce += Arrive();
            }

            if(On(SteeringBehavior.Pursuit)) {
                steeringForce += Pursuit();
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

        private Vector3 Pursuit()
        {
            var toEvader = _pursuitParams.target.GlobalTranslation - _owner.GlobalTranslation;

            // if the evader is ahead and facing us, we can just seek it
            // acos(0.95) = 18 degrees
            float relativeHeading = _owner.Heading.Dot(_pursuitParams.target.Heading);
            if(toEvader.Dot(_owner.Heading) > 0.0f && relativeHeading < -0.95f) {
                return Seek(_pursuitParams.target.GlobalTranslation, _pursuitParams.maxSpeed);
            }

            float lookAheadTime = toEvader.Length() / (_pursuitParams.maxSpeed + _pursuitParams.target.MaxSpeed);
            return Seek(_pursuitParams.target.GlobalTranslation + _pursuitParams.target.Velocity * lookAheadTime, _pursuitParams.maxSpeed);
        }

        private Vector3 Wander(float delta)
        {
            float jitter = _wanderParams.jitter * delta;

            // offset slightly on the circle
            _wanderParams.target += new Vector3(
                PartyParrotManager.Instance.Random.NextSingle(-1.0f, 1.0f) * jitter,
                0.0f,
                PartyParrotManager.Instance.Random.NextSingle(-1.0f, 1.0f) * jitter
            );

            var wanderTarget = _wanderParams.target.Normalized();
            _wanderParams.target = wanderTarget * _wanderParams.radius;

            var target = _wanderParams.target + (wanderTarget * _wanderParams.distance);
            return target;
        }

        // TODO: path follow (probably the only thing we actually need)

        #endregion
    }
}

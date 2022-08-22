using Godot;

using System;

using pdxpartyparrot.ssjAug2022.Managers;
using pdxpartyparrot.ssjAug2022.Util;

namespace pdxpartyparrot.ssjAug2022.NPCs.AI
{
    public class Steering<T> where T : SimpleNPC
    {
        public enum ArriveDeceleration
        {
            Fast = 1,

            Normal = 2,

            Slow = 3,
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

        const float DecelerationTweaker = 0.3f;

        private T _owner;

        private SteeringBehavior _enabledBehaviors = SteeringBehavior.None;

        private Vector3? _seekTarget;

        private Vector3? _arriveTarget;

        private ArriveDeceleration _arriveDeceleration = ArriveDeceleration.Normal;

        private SimpleCharacter _pursuitTarget;

        private Vector3 _wanderTarget;

        private float _wanderRadius;

        private float _wanderDistance;

        private float _wanderJitter;

        public Steering(T owner)
        {
            _owner = owner;
        }

        #region Enable / Disable

        public void SeekOn(Vector3 target)
        {
            _seekTarget = target;

            _enabledBehaviors |= SteeringBehavior.Seek;
        }

        public void SeekOff()
        {
            _seekTarget = null;

            _enabledBehaviors &= ~SteeringBehavior.Seek;
        }

        public void ArriveOn(Vector3 target, ArriveDeceleration deceleration = ArriveDeceleration.Normal)
        {
            _arriveTarget = target;
            _arriveDeceleration = deceleration;

            _enabledBehaviors |= SteeringBehavior.Arrive;
        }

        public void ArriveOff()
        {
            _arriveTarget = null;

            _enabledBehaviors &= ~SteeringBehavior.Arrive;
        }

        public void PursuitOn(SimpleCharacter target)
        {
            _pursuitTarget = target;

            _enabledBehaviors |= SteeringBehavior.Pursuit;
        }

        public void PursuitOff()
        {
            _pursuitTarget = null;

            _enabledBehaviors &= ~SteeringBehavior.Pursuit;
        }

        public void WanderOn(float wanderRadius = 1.0f, float wanderDistance = 1.0f, float wanderJitter = 0.5f)
        {
            _wanderRadius = wanderRadius;
            _wanderDistance = wanderDistance;
            _wanderJitter = wanderJitter;

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

        public Vector3 Calculate()
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
                steeringForce += Wander();
            }

            return steeringForce;
        }

        #region Steering Behaviors

        private Vector3 Seek()
        {
            if(!_seekTarget.HasValue) {
                return Vector3.Zero;
            }

            return Seek(_seekTarget.Value);
        }

        private Vector3 Seek(Vector3 target)
        {
            var desiredVelocity = (target - _owner.GlobalTranslation).Normalized() * _owner.Speed;
            return desiredVelocity - _owner.Velocity;
        }

        private Vector3 Arrive()
        {
            if(!_arriveTarget.HasValue) {
                return Vector3.Zero;
            }

            var toTarget = _arriveTarget.Value - _owner.GlobalTranslation;

            float distance = toTarget.Length();
            if(distance <= 0.0f) {
                return Vector3.Zero;
            }

            float speed = distance / ((int)_arriveDeceleration * DecelerationTweaker);
            speed = Math.Min(speed, _owner.Speed);

            var desiredVelocity = toTarget * speed / distance;
            return desiredVelocity - _owner.Velocity;
        }

        private Vector3 Pursuit()
        {
            if(_pursuitTarget == null) {
                return Vector3.Zero;
            }

            var toEvader = _pursuitTarget.GlobalTranslation - _owner.GlobalTranslation;
            float relativeHeading = _owner.Heading.Dot(_pursuitTarget.Heading);

            // if the evader is ahead and facing us, we can just seek it
            // acos(0.95) = 18 degrees
            if(toEvader.Dot(_owner.Heading) > 0.0f && relativeHeading < -0.95f) {
                return Seek(_pursuitTarget.GlobalTranslation);
            }

            float lookAheadTime = toEvader.Length() / (_owner.Speed + _pursuitTarget.Speed);
            return Seek(_pursuitTarget.GlobalTranslation + _pursuitTarget.Velocity * lookAheadTime);
        }

        private Vector3 Wander()
        {
            _wanderTarget += new Vector3(
                PartyParrotManager.Instance.Random.NextSingle(-1.0f, 1.0f) * _wanderJitter,
                0.0f,
                PartyParrotManager.Instance.Random.NextSingle(-1.0f, 1.0f) * _wanderJitter
            );

            _wanderTarget = _wanderTarget.Normalized() * _wanderRadius;

            var target = _owner.Transform.Xform(_wanderTarget + new Vector3(0.0f, 0.0f, _wanderDistance));
            return target - _owner.GlobalTranslation;
        }

        // TODO: path follow (probably the only thing we actually need)

        #endregion
    }
}

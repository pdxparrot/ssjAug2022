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

        const float DecelerationTweaker = 0.3f;

        private T _owner;

        private Vector3? _target;

        private ArriveDeceleration _arriveDeceleration = ArriveDeceleration.Normal;

        private Vector3 _wanderTarget;

        public float WanderRadius { get; set; } = 1.0f;

        public float WanderDistance { get; set; } = 1.0f;

        public float WanderJitter { get; set; } = 1.0f;

        private SimpleCharacter _pursuitTarget;

        public Steering(T owner)
        {
            _owner = owner;

            _wanderTarget = _owner.GlobalTranslation;
        }

        public Vector3 Calculate()
        {
            return Vector3.Zero;
        }

        private Vector3 Seek()
        {
            if(!_target.HasValue) {
                return Vector3.Zero;
            }

            return Seek(_target.Value);
        }

        private Vector3 Seek(Vector3 target)
        {
            var desiredVelocity = (target - _owner.GlobalTranslation).Normalized() * _owner.Speed;
            return desiredVelocity - _owner.Velocity;
        }

        private Vector3 Arrive()
        {
            if(!_target.HasValue) {
                return Vector3.Zero;
            }

            var toTarget = _target.Value - _owner.GlobalTranslation;

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
                PartyParrotManager.Instance.Random.NextSingle(-1.0f, 1.0f) * WanderJitter,
                0.0f,
                PartyParrotManager.Instance.Random.NextSingle(-1.0f, 1.0f) * WanderJitter
            );

            _wanderTarget = _wanderTarget.Normalized() * WanderRadius;

            var target = _owner.Transform.Xform(_wanderTarget + new Vector3(0.0f, 0.0f, WanderDistance));
            return target - _owner.GlobalTranslation;
        }
    }
}

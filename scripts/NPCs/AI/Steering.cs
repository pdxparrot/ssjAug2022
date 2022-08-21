using Godot;

namespace pdxpartyparrot.ssjAug2022.NPCs.AI
{
    public class Steering<T> where T : SimpleNPC
    {
        private T _owner;

        private Vector3? _seekTarget;

        public Steering(T owner)
        {
            _owner = owner;
        }

        public Vector3 Calculate()
        {
            return Vector3.Zero;
        }

        private Vector3 Seek()
        {
            if(_seekTarget.HasValue) {
                var desiredVelocity = (_seekTarget.Value - _owner.GlobalTranslation).Normalized() * _owner.Speed;
                return desiredVelocity - _owner.Velocity;
            }
            return Vector3.Zero;
        }
    }
}

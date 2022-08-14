using Godot;

namespace pdxpartyparrot.ssjAug2022
{
    public abstract class SimpleCharacter : KinematicBody
    {
        [Export]
        private float _speed = 14.0f;

        [Export]
        private float _gravityMultiplier = 5.0f;

        private Vector3 _velocity = Vector3.Zero;

        private Vector3 _heading = Vector3.Zero;

        protected Vector3 Heading
        {
            get => _heading;
            set => _heading = value;
        }

        private float _gravity;

        private Vector3 _gravityVector;

        private Spatial _pivot;

        #region Godot Lifecycle

        public override void _Ready()
        {
            _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
            _gravityVector = (Vector3)ProjectSettings.GetSetting("physics/3d/default_gravity_vector");

            _pivot = GetNode<Spatial>("Pivot");
        }

        public override void _PhysicsProcess(float delta)
        {
            // look in the direction we're heading
            if(Heading != Vector3.Zero) {
                Heading = Heading.Normalized();
                _pivot.LookAt(Translation + Heading, Vector3.Up);
            }

            // movement
            _velocity.x = Heading.x * _speed;
            _velocity.z = Heading.z * _speed;

            // gravity
            _velocity += _gravityVector * (_gravity * delta * _gravityMultiplier);

            // move the player
            _velocity = MoveAndSlide(_velocity, Vector3.Up);

            Heading = Vector3.Zero;
        }

        #endregion
    }
}

using Godot;

namespace pdxpartyparrot.ssjAug2022
{
    public class Player : KinematicBody
    {
        [Export]
        private float _maxSpeed = 14.0f;

        [Export]
        private float _gravityModifier = 75.0f;

        private Vector3 _velocity = Vector3.Zero;

        private Spatial _pivot;

        #region Godot Lifecycle

        public override void _Ready()
        {
            _pivot = GetNode<Spatial>("Pivot");
        }

        public override void _PhysicsProcess(float delta)
        {
            Vector3 heading = Vector3.Zero;

            if(Input.IsActionPressed("move_right")) {
                heading.x += 1.0f;
            }

            if(Input.IsActionPressed("move_left")) {
                heading.x -= 1.0f;
            }

            if(Input.IsActionPressed("move_forward")) {
                heading.z -= 1.0f;
            }

            if(Input.IsActionPressed("move_back")) {
                heading.z += 1.0f;
            }

            // look in the direction we're moving
            if(heading != Vector3.Zero) {
                heading = heading.Normalized();
                _pivot.LookAt(Translation + heading, Vector3.Up);
            }

            // movement
            _velocity.x = heading.x * _maxSpeed;
            _velocity.z = heading.z * _maxSpeed;

            // gravity mod
            _velocity.y -= _gravityModifier * delta;

            // move the player
            _velocity = MoveAndSlide(_velocity, Vector3.Up);
        }

        #endregion
    }
}

using Godot;

namespace pdxpartyparrot.ssjAug2022
{
    public class Player : KinematicBody
    {
        [Export]
        private float _speed = 14.0f;

        [Export]
        private float _gravity = 75.0f;

        private Vector3 _velocity = Vector3.Zero;

        #region Godot Lifecycle

        public override void _PhysicsProcess(float dt)
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
                GetNode<Spatial>("Pivot").LookAt(Translation + heading, Vector3.Up);
            }

            // movement
            _velocity.x = heading.x * _speed;
            _velocity.z = heading.z * _speed;

            // gravity
            _velocity.y -= _gravity * dt;

            // move the player
            _velocity = MoveAndSlide(_velocity, Vector3.Up);
        }

        #endregion
    }
}

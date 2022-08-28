using Godot;

using pdxpartyparrot.ssjAug2022.World;

namespace pdxpartyparrot.ssjAug2022
{
    public abstract class SimpleCharacter : KinematicBody
    {
        [Export]
        private float _mass = 1.0f;

        public float Mass => _mass;

        [Export]
        private float _maxSpeed = 14.0f;

        public float MaxSpeed
        {
            get => _maxSpeed;
            set => _maxSpeed = value;
        }

        [Export]
        private float _gravityMultiplier = 5.0f;

        private Vector3 _acceleration;

        private Vector3 _velocity;

        public Vector3 Velocity
        {
            get => _velocity;
            protected set => _velocity = value.LimitLength(MaxSpeed);
        }

        public float Speed => _velocity.Length();

        public float HorizontalSpeed => new Vector3(_velocity.x, 0.0f, _velocity.z).Length();

        private Vector3 _heading;

        public Vector3 Heading => _heading;

        private float _gravity;

        private Spatial _pivot;

        protected Spatial Pivot => _pivot;

        private Model _model;

        protected Model Model => _model;

        #region Godot Lifecycle

        public override void _Ready()
        {
            _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");

            _pivot = GetNode<Spatial>("Pivot");

            _model = _pivot.GetNode<Model>("Model");
            _model.UpdateMotionBlend(0.0f);
        }

        public override void _Process(float delta)
        {
            _model.UpdateMotionBlend(MaxSpeed > 0.0f ? HorizontalSpeed / MaxSpeed : 0.0f);
        }

        public override void _PhysicsProcess(float delta)
        {
            // apply gravity
            _acceleration += Vector3.Down * _gravity * _gravityMultiplier;

            // apply acceleration
            _velocity += _acceleration * delta;
            _acceleration = Vector3.Zero;

            // cap horizontal speed
            float y = _velocity.y;
            _velocity.y = 0.0f;
            _velocity = _velocity.LimitLength(_maxSpeed);
            _velocity.y = y;

            // calculate horizontal heading
            _heading = new Vector3(_velocity.x, 0.0f, _velocity.z);
            if(_heading != Vector3.Zero) {
                _heading = _heading.Normalized();

                // look in the direction we're heading
                _pivot.LookAt(Translation + _heading, Vector3.Up);
            }

            // move the player
            _velocity = MoveAndSlide(_velocity, Vector3.Up);
        }

        #endregion

        public void ApplyForce(Vector3 force)
        {
            if(_mass > 0.0f) {
                force /= _mass;
            }

            _acceleration += force;
        }

        #region Events

        public virtual void OnSpawn(SpawnPoint spawnPoint)
        {
            OnIdle();
        }

        public virtual void OnReSpawn(SpawnPoint spawnPoint)
        {
            OnIdle();
        }

        public virtual void OnDeSpawn()
        {
        }

        public virtual void OnIdle()
        {
        }

        #endregion
    }
}

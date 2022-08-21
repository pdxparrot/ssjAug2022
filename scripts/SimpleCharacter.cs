using Godot;

using pdxpartyparrot.ssjAug2022.World;

namespace pdxpartyparrot.ssjAug2022
{
    public abstract class SimpleCharacter : KinematicBody
    {
        [Export]
        private float _mass = 1.0f;

        [Export]
        private float _speed = 14.0f;

        public float Speed
        {
            get => _speed;
            set => _speed = value;
        }

        [Export]
        private float _gravityMultiplier = 5.0f;

        private Vector3 _velocity = Vector3.Zero;

        public Vector3 Velocity => _velocity;

        private Vector3 _heading = Vector3.Zero;

        protected Vector3 Heading
        {
            get => _heading;
            set => _heading = value;
        }

        public bool IsInputAllowed { get; set; } = true;

        private float _gravity;

        private Vector3 _gravityVector;

        private Spatial _pivot;

        protected Spatial Pivot => _pivot;

        private Model _model;

        protected Model Model
        {
            get => _model;
        }

        #region Godot Lifecycle

        public override void _Ready()
        {
            _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
            _gravityVector = (Vector3)ProjectSettings.GetSetting("physics/3d/default_gravity_vector");

            _pivot = GetNode<Spatial>("Pivot");
            _model = _pivot.GetNode<Model>("Model");
        }

        public override void _Process(float delta)
        {
            _model.UpdateMotionBlend(Heading.Length());
        }

        public override void _PhysicsProcess(float delta)
        {
            // look in the direction we're heading
            if(Heading != Vector3.Zero) {
                _pivot.LookAt(Translation + Heading, Vector3.Up);
            }

            // movement
            _velocity.x = Heading.x * _speed;
            _velocity.z = Heading.z * _speed;

            // gravity
            _velocity += _gravityVector * (_gravity * delta * _gravityMultiplier);

            // move the player
            _velocity = MoveAndSlide(_velocity, Vector3.Up);
        }

        #endregion

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

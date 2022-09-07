using Godot;

using System;

using pdxpartyparrot.ssjAug2022.Managers;
using pdxpartyparrot.ssjAug2022.Util;
using pdxpartyparrot.ssjAug2022.World;

namespace pdxpartyparrot.ssjAug2022
{
    public abstract class SimpleCharacter : KinematicBody, IDebugDraw
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
            protected set => _velocity = LimitVelocity(value);
        }

        public float Speed => _velocity.Length();

        public float HorizontalSpeed => new Vector3(_velocity.x, 0.0f, _velocity.z).Length();

        public Vector3 Forward => -Pivot.Transform.basis.z;

        public Vector3 Heading { get; private set; }

        public Vector3 Side { get; private set; } = Vector3.Right;

        private float _gravity;

        public Spatial Pivot { get; private set; }

        protected Model Model { get; private set; }

        #region Godot Lifecycle

        public override void _Ready()
        {
            _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");

            Pivot = GetNode<Spatial>("Pivot");

            Model = Pivot.GetNode<Model>("Model");
            Model.UpdateMotionBlend(0.0f);
        }

        public override void _EnterTree()
        {
            DebugOverlay.Instance.RegisterDebugDraw(this);

            GameManager.Instance.GameOverEvent += GameOverEventHandler;
        }

        public override void _ExitTree()
        {
            if(DebugOverlay.HasInstance) {
                DebugOverlay.Instance.UnRegisterDebugDraw(this);
            }

            if(GameManager.HasInstance) {
                GameManager.Instance.GameOverEvent -= GameOverEventHandler;
            }
        }

        public override void _Process(float delta)
        {
            Model.UpdateMotionBlend(MaxSpeed > 0.0f ? HorizontalSpeed / MaxSpeed : 0.0f);
        }

        public override void _PhysicsProcess(float delta)
        {
            if(PartyParrotManager.Instance.IsPaused) {
                return;
            }

            // TODO: a max turn rate and smoothed heading might make this nicer

            // apply gravity
            _acceleration += Vector3.Down * _gravity * _gravityMultiplier;

            // apply acceleration
            _velocity += _acceleration * delta;
            _acceleration = Vector3.Zero;

            // cap horizontal speed
            _velocity = LimitVelocity(_velocity);

            // calculate horizontal heading
            Heading = new Vector3(_velocity.x, 0.0f, _velocity.z);
            if(Heading.LengthSquared() > 0.01) {
                Heading = Heading.Normalized();

                // look in the direction we're heading
                Pivot.LookAt(Translation + Heading, Vector3.Up);
            } else {
                Heading = Forward;
            }

            Side = Heading.Perpendicular();

            // move the player
            _velocity = MoveAndSlide(_velocity, Vector3.Up);
        }

        #endregion

        public virtual void DebugDraw(CanvasItem cavas, Godot.Camera camera)
        {
            var start = camera.UnprojectPosition(GlobalTransform.origin);

            var velocity = camera.UnprojectPosition(GlobalTransform.origin + Velocity);
            cavas.DrawLine(start, velocity, new Color(1.0f, 1.0f, 1.0f), 2.0f);

            var heading = camera.UnprojectPosition(GlobalTransform.origin + Heading);
            cavas.DrawLine(start, heading, new Color(0.0f, 0.0f, 1.0f), 2.0f);

            var side = camera.UnprojectPosition(GlobalTransform.origin + Side);
            cavas.DrawLine(start, side, new Color(0.0f, 1.0f, 0.0f), 2.0f);
        }

        public virtual void Stop()
        {
            Velocity = new Vector3(0.0f, Velocity.y, 0.0f);
        }

        public void ApplyForce(Vector3 force)
        {
            if(_mass > 0.0f) {
                force /= _mass;
            }

            _acceleration += force;
        }

        protected Vector3 LimitVelocity(Vector3 velocity)
        {
            float y = velocity.y;
            velocity.y = 0.0f;
            velocity = velocity.LimitLength(_maxSpeed);
            velocity.y = y;
            return velocity;
        }

        #region Spawn

        public virtual void OnSpawn(SpawnPoint spawnPoint)
        {
            // spawnpoint rotates the main object
            // but what we actually want to rotate is the pivot
            Pivot.Rotation = Rotation;
            Rotation = Vector3.Zero;

            OnIdle();
        }

        public virtual void OnReSpawn(SpawnPoint spawnPoint)
        {
            OnIdle();
        }

        public virtual void OnDeSpawn()
        {
        }

        #endregion

        #region Events

        public virtual void OnIdle()
        {
        }

        #endregion

        #region Event Handlers

        private void GameOverEventHandler(object sender, EventArgs args)
        {
            Stop();
        }

        #endregion
    }
}

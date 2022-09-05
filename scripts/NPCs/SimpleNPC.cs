using Godot;

using System;

using pdxpartyparrot.ssjAug2022.Managers;
using pdxpartyparrot.ssjAug2022.NPCs.AI;
using pdxpartyparrot.ssjAug2022.World;

namespace pdxpartyparrot.ssjAug2022.NPCs
{
    public abstract class SimpleNPC : SimpleCharacter
    {
        [Export]
        private float _maxTurnRate = Mathf.Pi / 4.0f;

        public float MaxTurnRate => _maxTurnRate;

        private NavigationAgent _agent;

        public Guid Id { get; set; }

        #region Godot Lifecycle

        public override void _Ready()
        {
            base._Ready();

            _agent = GetNode<NavigationAgent>("NavigationAgent");
        }

        #endregion

        public virtual bool HandleMessage(Telegram message)
        {
            return false;
        }

        public override void Stop()
        {
            base.Stop();

            SetVelocity(Velocity);
        }

        #region Navigation

        public void SetTarget(Vector3 target)
        {
            _agent.SetTargetLocation(target);
        }

        public bool IsNavigationFinished()
        {
            return _agent.IsNavigationFinished();
        }

        public bool IsTargetReachable()
        {
            return _agent.IsTargetReachable();
        }

        public bool IsTargetReached()
        {
            return _agent.IsTargetReached();
        }

        public Vector3 GetNextLocation()
        {
            return _agent.GetNextLocation();
        }

        public Vector3 GetFinalLocation()
        {
            return _agent.GetFinalLocation();
        }

        public float DistanceToTarget()
        {
            return _agent.DistanceToTarget();
        }

        public void SetVelocity(Vector3 velocity)
        {
            velocity = LimitVelocity(velocity);
            _agent.SetVelocity(velocity);
        }

        protected virtual void UpdateVelocity(Vector3 velocity)
        {
            Velocity = velocity;
        }

        private Vector3 LimitTurnRate(Vector3 velocity)
        {
            /*var current = new Vector2(Velocity.x, Velocity.z);
            if(current.LengthSquared() < 0.01) {
                current = new Vector2(Forward.x, Forward.z);
            }

            var desired = new Vector2(velocity.x, velocity.z);

            float angle = Vector2.Down.AngleTo(desired);
            var updated = current.Rotated(Math.Min(angle, _maxTurnRate)).Normalized() * desired.Length();

            return new Vector3(updated.x, velocity.y, updated.y);*/

            return velocity;
        }

        #endregion

        #region Spawn

        public override void OnSpawn(SpawnPoint spawnPoint)
        {
            base.OnSpawn(spawnPoint);

            NPCManager.Instance.RegisterNPC(this);
        }

        public override void OnReSpawn(SpawnPoint spawnPoint)
        {
            base.OnReSpawn(spawnPoint);

            NPCManager.Instance.RegisterNPC(this);
        }

        public override void OnDeSpawn()
        {
            NPCManager.Instance.UnRegisterNPC(this);

            base.OnDeSpawn();
        }

        #endregion

        #region Signal Handlers

        protected void _on_NavigationAgent_velocity_computed(Vector3 safeVelocity)
        {
            //GD.Print($"[{Id}] velocity updated: {safeVelocity}");

            var velocity = LimitTurnRate(new Vector3(safeVelocity.x, Velocity.y, safeVelocity.z));
            //GD.Print($"[{Id}] turn rate limited velocity: {velocity}");

            UpdateVelocity(velocity);
        }

        #endregion
    }
}

using Godot;

using System;

using pdxpartyparrot.ssjAug2022.Managers;
using pdxpartyparrot.ssjAug2022.NPCs.AI;
using pdxpartyparrot.ssjAug2022.World;

namespace pdxpartyparrot.ssjAug2022.NPCs
{
    public abstract class SimpleNPC : SimpleCharacter
    {
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

        public void SetVelocity(Vector3 velocity)
        {
            velocity = LimitVelocity(velocity);
            _agent.SetVelocity(velocity);
        }

        protected virtual void UpdateVelocity(Vector3 velocity)
        {
            Velocity = velocity;
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
            UpdateVelocity(new Vector3(safeVelocity.x, Velocity.y, safeVelocity.z));
        }

        #endregion
    }
}

using Godot;

using System;

using pdxpartyparrot.ssjAug2022.Managers;
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
            _agent.SetVelocity(velocity);
        }

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
            Velocity = new Vector3(safeVelocity.x, 0.0f, safeVelocity.z);
        }

        #endregion
    }
}

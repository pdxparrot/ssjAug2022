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
    }
}

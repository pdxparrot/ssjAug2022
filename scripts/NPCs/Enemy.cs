using Godot;

using System;

using pdxpartyparrot.ssjAug2022.Interactables;
using pdxpartyparrot.ssjAug2022.World;

namespace pdxpartyparrot.ssjAug2022.NPCs
{
    public class Enemy : SimpleNPC, IInteractable
    {
        [Export]
        private int _maxHealth = 1;

        public int MaxHealth => _maxHealth;

        public int CurrentHealth { get; protected set; }

        [Export]
        private float _wanderSpeed = 5.0f;

        public float WanderSpeed => _wanderSpeed;

        [Export]
        private float _idleLeashRange = 10.0f;

        public float IdleLeashRangeSquared => _idleLeashRange * _idleLeashRange;

        public bool IsDead => CurrentHealth <= 0;

        public Vector3 HomeTranslation { get; private set; }

        public bool CanInteract => !IsDead;

        public Type InteractableType => GetType();

        #region Godot Lifecycle

        public override void _Ready()
        {
            base._Ready();

            CurrentHealth = _maxHealth;
        }

        #endregion

        public void Kill()
        {
            Damage(CurrentHealth);
        }

        protected virtual void OnDied()
        {
            Stop();
        }

        public virtual void Damage(int amount)
        {
            if(IsDead) {
                return;
            }

            CurrentHealth = Mathf.Max(CurrentHealth - amount, 0);
            if(IsDead) {
                OnDied();
            }
        }

        #region Spawn

        public override void OnSpawn(SpawnPoint spawnPoint)
        {
            base.OnSpawn(spawnPoint);

            HomeTranslation = GlobalTranslation;
        }

        public override void OnReSpawn(SpawnPoint spawnPoint)
        {
            base.OnReSpawn(spawnPoint);

            HomeTranslation = GlobalTranslation;
        }

        #endregion
    }
}

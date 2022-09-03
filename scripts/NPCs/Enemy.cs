using Godot;

using System;

using pdxpartyparrot.ssjAug2022.Interactables;

namespace pdxpartyparrot.ssjAug2022.NPCs
{
    public class Enemy : SimpleNPC, IInteractable
    {
        [Export]
        private int _maxHealth = 1;

        public int MaxHealth => _maxHealth;

        private int _currentHealth;

        public int CurrentHealth => _currentHealth;

        public bool IsDead => _currentHealth <= 0;

        public bool CanInteract => !IsDead;

        public Type InteractableType => GetType();

        #region Godot Lifecycle

        public override void _Ready()
        {
            base._Ready();

            _currentHealth = _maxHealth;
        }

        #endregion

        public void Kill()
        {
            Damage(_currentHealth);
        }

        protected virtual void OnDied()
        {
            Stop();
        }

        public void Damage(int amount)
        {
            if(IsDead) {
                return;
            }

            _currentHealth = Mathf.Max(_currentHealth - amount, 0);
            if(IsDead) {
                OnDied();
            }
        }
    }
}

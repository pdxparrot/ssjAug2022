using Godot;

namespace pdxpartyparrot.ssjAug2022
{
    public class Human : SimpleNPC
    {
        [Export]
        private int _maxHealth = 1;

        private int _currentHealth;

        public bool IsDead => _currentHealth <= 0;

        #region Godot Lifecycle

        public override void _Ready()
        {
            _currentHealth = _maxHealth;

            base._Ready();
        }

        #endregion

        public void Damage(int amount)
        {
            _currentHealth = Mathf.Max(_currentHealth - amount, 0);
            if(IsDead) {
                // TODO: despawn
            }
        }
    }
}

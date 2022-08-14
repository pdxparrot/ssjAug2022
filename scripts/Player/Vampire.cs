using Godot;

using pdxpartyparrot.ssjAug2022.Managers;

namespace pdxpartyparrot.ssjAug2022.Player
{
    public class Vampire : SimplePlayer
    {
        [Export]
        private int _maxHealth = 10;

        private int _currentHealth;

        public bool IsDead => _currentHealth <= 0;

        #region Godot Lifecycle

        public override void _Ready()
        {
            _currentHealth = _maxHealth;

            base._Ready();
        }

        public override void _PhysicsProcess(float delta)
        {
            var heading = Heading;

            if(Input.IsActionPressed("move_right")) {
                heading.x += 1.0f;
            }

            if(Input.IsActionPressed("move_left")) {
                heading.x -= 1.0f;
            }

            if(Input.IsActionPressed("move_forward")) {
                heading.z -= 1.0f;
            }

            if(Input.IsActionPressed("move_back")) {
                heading.z += 1.0f;
            }

            Heading = heading;

            base._PhysicsProcess(delta);
        }

        #endregion

        public void Damage(int amount)
        {
            _currentHealth = Mathf.Max(_currentHealth - amount, 0);
            if(IsDead) {
                GameManager.Instance.GameOver();
            }
        }
    }
}

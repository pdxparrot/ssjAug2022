using Godot;

using System.Threading.Tasks;

using pdxpartyparrot.ssjAug2022.Collections;
using pdxpartyparrot.ssjAug2022.Managers;
using pdxpartyparrot.ssjAug2022.NPCs;
using pdxpartyparrot.ssjAug2022.World;

namespace pdxpartyparrot.ssjAug2022.Player
{
    public class Vampire : SimplePlayer
    {
        [Export]
        private int _maxHealth = 10;

        private int _currentHealth;

        public bool IsDead => _currentHealth <= 0;

        #region Claw Attack

        [Export]
        private float _clawAttackRange = 5.0f;

        [Export]
        private int _clawAttackDamage = 1;

        // TODO: this would be better if it was driven by the animation
        private Timer _clawTimer;

        private Timer _clawCooldown;

        #endregion

        #region Dash

        private Timer _dashTimer;

        private Timer _dashCooldown;

        #endregion

        #region Godot Lifecycle

        public override void _Ready()
        {
            _currentHealth = _maxHealth;

            _clawTimer = GetNode<Timer>("Timers/Claw Timer");
            _clawCooldown = GetNode<Timer>("Timers/Claw Cooldown");

            _dashTimer = GetNode<Timer>("Timers/Dash Timer");
            _dashCooldown = GetNode<Timer>("Timers/Dash Cooldown");

            base._Ready();
        }

        public override async void _Input(InputEvent @event)
        {
            if(!InputAllowed) {
                return;
            }

            if(@event.IsActionPressed("claw_attack")) {
                await ClawAttackAsync();
            } else if(@event.IsActionPressed("dash")) {
                Dash();
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            if(InputAllowed) {
                var heading = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
                Heading = new Vector3(heading.x, 0.0f, heading.y);
            }

            base._PhysicsProcess(delta);
        }

        #endregion

        public async Task DamageAsync(int amount)
        {
            _currentHealth = Mathf.Max(_currentHealth - amount, 0);
            GameUIManager.Instance.HUD.UpdateHealth(_currentHealth);

            if(IsDead) {
                await GameManager.Instance.GameOverAsync().ConfigureAwait(false);
            }
        }

        public async Task ClawAttackAsync()
        {
            if(!_clawTimer.IsStopped() || !_clawCooldown.IsStopped()) {
                return;
            }

            GD.Print("[Player] Claw attack!");
            Model.TriggerOneShot("parameters/Claw_AttackTrigger/active");

            var enemy = (Human)NPCManager.Instance.NPCs.NearestManhattan(GlobalTranslation, out float distance);
            if(enemy != null && distance <= _clawAttackRange) {
                await enemy.DamageAsync(_clawAttackDamage);
            }

            _clawTimer.Start();
        }

        public void Dash()
        {
            if(!_dashTimer.IsStopped() || !_dashCooldown.IsStopped()) {
                return;
            }

            GD.Print("[Player] Dash!");
            //Model.TriggerOneShot("parameters/Dash_Trigger/active");

            _dashTimer.Start();
        }

        #region Signal Handlers

        private void _on_Claw_Timer_timeout()
        {
            _clawCooldown.Start();
        }

        private void _on_Dash_Timer_timeout()
        {
            _dashCooldown.Start();
        }

        #endregion

        #region Events

        public override void OnSpawn(SpawnPoint spawnPoint)
        {
            base.OnSpawn(spawnPoint);

            _currentHealth = _maxHealth;

            GameUIManager.Instance.HUD.SetMaxHealth(_maxHealth);
            GameUIManager.Instance.HUD.UpdateHealth(_currentHealth);
        }

        public override void OnReSpawn(SpawnPoint spawnPoint)
        {
            base.OnSpawn(spawnPoint);

            _currentHealth = _maxHealth;

            GameUIManager.Instance.HUD.SetMaxHealth(_maxHealth);
            GameUIManager.Instance.HUD.UpdateHealth(_currentHealth);
        }

        #endregion
    }
}

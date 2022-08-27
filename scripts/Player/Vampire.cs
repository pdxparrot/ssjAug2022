using Godot;

using System.Linq;
using System.Threading.Tasks;

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
        private int _clawAttackDamage = 1;

        private Interactables.Interactables _clawAttackInteractables;

        // TODO: this would be better if it was driven by the animation
        private Timer _clawTimer;

        private Timer _clawCooldown;

        private AudioStreamPlayer _clawAttackAudioPlayer;

        #endregion

        #region Power Unleashed

        [Export]
        private int _powerUnleashedDamage = 1;

        // TODO: this would be better if it was driven by the animation
        private Timer _powerUnleashedTimer;

        private Timer _powerUnleashedCooldown;

        private Interactables.Interactables _powerUnleashedInteractables;

        #endregion

        #region Dash

        [Export]
        private float _dashModifier = 3.0f;

        private Timer _dashTimer;

        private Timer _dashCooldown;

        private AudioStreamPlayer _dashAudioPlayer;

        #endregion

        #region Godot Lifecycle

        public override void _Ready()
        {
            base._Ready();

            _currentHealth = _maxHealth;

            _clawTimer = GetNode<Timer>("Timers/Claw Timer");
            _clawCooldown = GetNode<Timer>("Timers/Claw Cooldown");
            _clawAttackInteractables = Pivot.GetNode<Interactables.Interactables>("ClawAttack Hitbox");
            _clawAttackAudioPlayer = GetNode<AudioStreamPlayer>("SFX/ClawAttack");

            _powerUnleashedTimer = GetNode<Timer>("Timers/Power Unleashed Timer");
            _powerUnleashedCooldown = GetNode<Timer>("Timers/Power Unleashed Cooldown");
            _powerUnleashedInteractables = Pivot.GetNode<Interactables.Interactables>("PowerUnleashed Hitbox");

            _dashTimer = GetNode<Timer>("Timers/Dash Timer");
            _dashCooldown = GetNode<Timer>("Timers/Dash Cooldown");
            _dashAudioPlayer = GetNode<AudioStreamPlayer>("SFX/Dash");
        }

        public override async void _Input(InputEvent @event)
        {
            if(!IsInputAllowed) {
                return;
            }

            if(@event.IsActionPressed("claw_attack")) {
                ClawAttack();
            } else if(@event.IsAction("power_unleashed")) {
                await PowerUnleashedAsync().ConfigureAwait(false);
            } else if(@event.IsActionPressed("dash")) {
                Dash();
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            if(IsInputAllowed) {
                var input = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
                Velocity = new Vector3(input.x, 0.0f, input.y) * MaxSpeed;
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

        private async Task DamageInteractableEnemeiesAsync(Interactables.Interactables interactables, int damage)
        {
            var enemies = interactables.GetInteractables<Human>();

            // copy because we're going to modify the underlying collection
            var humans = new Human[enemies.Count];
            for(int idx = 0; idx < enemies.Count; ++idx) {
                humans[idx] = (Human)enemies.ElementAt(idx);
            }

            foreach(var human in humans) {
                await human.DamageAsync(damage).ConfigureAwait(false);
            }
        }

        private async Task DoClawAttackDamageAsync()
        {
            await DamageInteractableEnemeiesAsync(_clawAttackInteractables, _clawAttackDamage).ConfigureAwait(false);
        }

        public void ClawAttack()
        {
            if(!_clawTimer.IsStopped() || !_clawCooldown.IsStopped()) {
                return;
            }

            GD.Print("[Player] Claw attack!");
            Model.TriggerOneShot("parameters/Claw_AttackTrigger/active");
            _clawAttackAudioPlayer.Play();

            _clawTimer.Start();
        }

        private async Task DoPowerUnleashedDamageAsync()
        {
            await DamageInteractableEnemeiesAsync(_powerUnleashedInteractables, _powerUnleashedDamage).ConfigureAwait(false);
        }

        public async Task PowerUnleashedAsync()
        {
            if(!_powerUnleashedTimer.IsStopped() || !_powerUnleashedCooldown.IsStopped()) {
                return;
            }

            GD.Print("[Player] Power unleashed!");
            //Model.TriggerOneShot("parameters/Power_UnleashedTrigger/active");

            _powerUnleashedTimer.Start();

            await DoPowerUnleashedDamageAsync().ConfigureAwait(false);
        }

        public void Dash()
        {
            if(!_dashTimer.IsStopped() || !_dashCooldown.IsStopped()) {
                return;
            }

            GD.Print("[Player] Dash!");
            //Model.TriggerOneShot("parameters/Dash_Trigger/active");
            _dashAudioPlayer.Play();

            IsInputAllowed = false;

            MaxSpeed *= _dashModifier;
            Velocity = -Pivot.Transform.basis.z * MaxSpeed;

            _dashTimer.Start();
        }

        #region Signal Handlers

        // TODO: this isn't firing :(
        private void _on_Claw_Attack_damage()
        {
            GD.Print($"Claw attack damage");
        }

        // TODO: this isn't firing :(
        private void _on_Claw_Attack_animation_finished()
        {
            GD.Print($"Claw attack finished");
        }

        private async void _on_Claw_Timer_timeout()
        {
            await DoClawAttackDamageAsync().ConfigureAwait(false);

            _clawCooldown.Start();
        }

        private void _on_Power_Unleashed_Timer_timeout()
        {
            _powerUnleashedCooldown.Start();
        }

        private void _on_Dash_Timer_timeout()
        {
            MaxSpeed /= _dashModifier;
            IsInputAllowed = true;

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

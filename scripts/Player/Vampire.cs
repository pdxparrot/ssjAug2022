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
        private Timer _clawAttackAnimationTimer;

        private Timer _clawAttackCooldown;

        private AudioStreamPlayer _clawAttackAudioPlayer;

        #endregion

        #region Power Unleashed

        [Export]
        private int _powerUnleashedDamage = 1;

        [Export]
        private float _powerUnleashedScale = 1.5f;

        private Interactables.Interactables _powerUnleashedInteractables;

        private VFX _powerUnleashedVFX;

        // TODO: this would be better if it was driven by the animation
        private Timer _powerUnleashedDelayTimer;

        // TODO: this would be better if it was driven by the animation
        private Timer _powerUnleashedScaleTimer;

        private Timer _powerUnleashedCooldown;

        private Vector3 _powerUnleashedInitialScale;

        private Vector3 _powerUnleashedMaxScale;

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

            _clawAttackAnimationTimer = GetNode<Timer>("Timers/ClawAttack Animation Timer");
            _clawAttackCooldown = GetNode<Timer>("Timers/ClawAttack Cooldown");
            _clawAttackInteractables = Pivot.GetNode<Interactables.Interactables>("ClawAttack Hitbox");
            _clawAttackAudioPlayer = GetNode<AudioStreamPlayer>("SFX/ClawAttack");

            _powerUnleashedDelayTimer = GetNode<Timer>("Timers/PowerUnleashed Delay Timer");
            _powerUnleashedScaleTimer = GetNode<Timer>("Timers/PowerUnleashed Scale Timer");
            _powerUnleashedCooldown = GetNode<Timer>("Timers/PowerUnleashed Cooldown");
            _powerUnleashedInteractables = Pivot.GetNode<Interactables.Interactables>("PowerUnleashed Hitbox");
            _powerUnleashedVFX = Pivot.GetNode<VFX>("PowerUnleashed VFX");
            _powerUnleashedInitialScale = _powerUnleashedInteractables.Scale;
            _powerUnleashedMaxScale = _powerUnleashedInitialScale * _powerUnleashedScale;

            _dashTimer = GetNode<Timer>("Timers/Dash Timer");
            _dashCooldown = GetNode<Timer>("Timers/Dash Cooldown");
            _dashAudioPlayer = GetNode<AudioStreamPlayer>("SFX/Dash");
        }

        public override void _Input(InputEvent @event)
        {
            if(!IsInputAllowed) {
                return;
            }

            if(@event.IsActionPressed("claw_attack")) {
                ClawAttack();
            } else if(@event.IsAction("power_unleashed")) {
                PowerUnleashed();
            } else if(@event.IsActionPressed("dash")) {
                Dash();
            }
        }

        public override async void _Process(float delta)
        {
            base._Process(delta);

            if(!_powerUnleashedScaleTimer.IsStopped()) {
                float pct = 1.0f - (_powerUnleashedScaleTimer.TimeLeft / _powerUnleashedScaleTimer.WaitTime);
                var scale = _powerUnleashedInitialScale + (_powerUnleashedMaxScale - _powerUnleashedInitialScale) * pct;
                scale.y = _powerUnleashedInitialScale.y;
                _powerUnleashedInteractables.Scale = scale;

                // TODO: we should pulse at a slower rate than every frame
                await DoPowerUnleashedDamageAsync().ConfigureAwait(false);
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
            if(!_clawAttackAnimationTimer.IsStopped() || !_clawAttackCooldown.IsStopped()) {
                return;
            }

            GD.Print("[Player] Claw attack!");
            Model.TriggerOneShot("parameters/Claw_AttackTrigger/active");
            _clawAttackAudioPlayer.Play();

            _clawAttackAnimationTimer.Start();
        }

        private async Task DoPowerUnleashedDamageAsync()
        {
            await DamageInteractableEnemeiesAsync(_powerUnleashedInteractables, _powerUnleashedDamage).ConfigureAwait(false);
        }

        public void PowerUnleashed()
        {
            if(!_powerUnleashedDelayTimer.IsStopped() || !_powerUnleashedScaleTimer.IsStopped() || !_powerUnleashedCooldown.IsStopped()) {
                return;
            }

            GD.Print("[Player] Power unleashed!");
            //Model.TriggerOneShot("parameters/Power_UnleashedTrigger/active");

            _powerUnleashedDelayTimer.Start();
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

        private async void _on_ClawAttack_Animation_Timer_timeout()
        {
            await DoClawAttackDamageAsync().ConfigureAwait(false);

            _clawAttackCooldown.Start();
        }

        private void _on_PowerUnleashed_Delay_Timer_timeout()
        {
            _powerUnleashedVFX.Play("power_unleashed");

            _powerUnleashedScaleTimer.Start();
        }

        private void _on_PowerUnleashed_Scale_Timer_timeout()
        {
            _powerUnleashedInteractables.Scale = _powerUnleashedInitialScale;

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

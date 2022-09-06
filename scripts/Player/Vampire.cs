using Godot;

using System;
using System.Linq;

using pdxpartyparrot.ssjAug2022.Interactables;
using pdxpartyparrot.ssjAug2022.Managers;
using pdxpartyparrot.ssjAug2022.NPCs;
using pdxpartyparrot.ssjAug2022.NPCs.Boss;
using pdxpartyparrot.ssjAug2022.NPCs.Human;
using pdxpartyparrot.ssjAug2022.World;

namespace pdxpartyparrot.ssjAug2022.Player
{
    public class Vampire : SimplePlayer, IInteractable
    {
        [Export]
        private int _maxHealth = 10;

        private int _currentHealth;

        public bool IsDead => _currentHealth <= 0;

        #region Claw Attack

        [Export]
        private int _clawAttackDamage = 1;

        private Interactables.Interactables _clawAttackInteractables;

        private VFX _clawAttackVFX;

        // TODO: this would be better if it was driven by the animation
        private Timer _clawAttackAnimationTimer;

        private Timer _clawAttackCooldown;

        private AudioStreamPlayer _clawAttackAudioPlayer;

        private bool CanClawAttack => !IsGlobalCooldown && _clawAttackAnimationTimer.IsStopped() && _clawAttackCooldown.IsStopped();

        #endregion

        #region Power Unleashed

        [Export]
        private bool _shouldPowerUnleashedRoot;

        [Export]
        private bool _dashCancelPowerUnleashed;

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

        private bool IsPowerUnleashing => !_powerUnleashedDelayTimer.IsStopped() || !_powerUnleashedScaleTimer.IsStopped();

        private bool IsPowerUnleashScaling => !_powerUnleashedScaleTimer.IsStopped();

        private float PowerUnleashedPercent => _powerUnleashedScaleTimer.TimeLeft / _powerUnleashedScaleTimer.WaitTime;

        private bool CanPowerUnleashed => !IsGlobalCooldown && !IsPowerUnleashing && _powerUnleashedCooldown.IsStopped();

        #endregion

        #region Dash

        [Export]
        private float _dashModifier = 3.0f;

        private Timer _dashTimer;

        private Timer _dashCooldown;

        private AudioStreamPlayer _dashAudioPlayer;

        private bool IsDashing => !_dashTimer.IsStopped();

        private bool CanDash => !IsGlobalCooldown && _dashTimer.IsStopped() && _dashCooldown.IsStopped();

        #endregion

        private Timer _deathTimer;

        public bool CanInteract => !IsDead;

        public Type InteractableType => GetType();

        private bool IsInputAllowed => !IsDead && !GameManager.Instance.IsGameOver && !PartyParrotManager.Instance.IsPaused && !IsDashing;

        private bool IsGlobalCooldown => !_clawAttackAnimationTimer.IsStopped() || IsPowerUnleashing || IsDashing;

        private bool IsRooted => _shouldPowerUnleashedRoot && IsPowerUnleashing;

        #region Godot Lifecycle

        public override void _Ready()
        {
            base._Ready();

            _currentHealth = _maxHealth;

            _clawAttackAnimationTimer = GetNode<Timer>("Timers/ClawAttack Animation Timer");
            _clawAttackCooldown = GetNode<Timer>("Timers/ClawAttack Cooldown");
            _clawAttackInteractables = Pivot.GetNode<Interactables.Interactables>("ClawAttack Hitbox");
            _clawAttackVFX = Pivot.GetNode<VFX>("ClawAttack VFX");
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

            _deathTimer = GetNode<Timer>("Timers/Death Timer");
        }

        public override void _ExitTree()
        {
            base._ExitTree();

            if(GameManager.HasInstance && GameManager.Instance.Level != null) {
                GameManager.Instance.Level.StageChangeEvent -= StageChangeEventHandler;
            }
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

        public override void _Process(float delta)
        {
            base._Process(delta);

            if(IsPowerUnleashScaling) {
                float pct = 1.0f - PowerUnleashedPercent;
                var scale = _powerUnleashedInitialScale + (_powerUnleashedMaxScale - _powerUnleashedInitialScale) * pct;
                scale.y = _powerUnleashedInitialScale.y;
                _powerUnleashedInteractables.Scale = scale;

                // TODO: we should pulse at a slower rate than every frame
                DoPowerUnleashedDamage();
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            if(IsRooted) {
                Velocity = new Vector3(0.0f, Velocity.y, 0.0f);
            } else if(IsInputAllowed) {
                var input = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
                Velocity = new Vector3(input.x, Velocity.y, input.y) * MaxSpeed;
            }

            base._PhysicsProcess(delta);
        }

        #endregion

        private void ResetHealth()
        {
            _currentHealth = _maxHealth;

            GameUIManager.Instance.HUD.UpdatePlayerHealth(1.0f);
        }

        public void Kill()
        {
            Damage(_currentHealth);
        }

        private void OnDied()
        {
            Stop();

            Model.ChangeState("death");

            // TODO: this should despawn the player but not destroy it
            _deathTimer.Start();
        }

        public void Damage(int amount)
        {
            if(IsDead) {
                return;
            }

            _currentHealth = Mathf.Max(_currentHealth - amount, 0);
            GameUIManager.Instance.HUD.UpdatePlayerHealth(_maxHealth > 0 ? _currentHealth / (float)_maxHealth : 0.0f);

            if(IsDead) {
                OnDied();
            }
        }

        private void DamageEnemies<T>(Interactables.Interactables interactables, int damage) where T : Enemy
        {
            var enemies = interactables.GetInteractables<T>();

            // copy because we're going to modify the underlying collection
            var copy = new T[enemies.Count];
            for(int idx = 0; idx < enemies.Count; ++idx) {
                copy[idx] = (T)enemies.ElementAt(idx);
            }

            foreach(var enemy in copy) {
                enemy.Damage(damage);
            }
        }

        private void DamageInteractableEnemeies(Interactables.Interactables interactables, int damage)
        {
            DamageEnemies<Human>(interactables, damage);
            DamageEnemies<Boss>(interactables, damage);
        }

        private void DoClawAttackDamage()
        {
            DamageInteractableEnemeies(_clawAttackInteractables, _clawAttackDamage);
        }

        private void ClawAttack()
        {
            if(!CanClawAttack) {
                return;
            }

            GD.Print($"[{Name}] Claw attack!");
            Model.ChangeState("claw_attack");
            _clawAttackAudioPlayer.Play();

            _clawAttackAnimationTimer.Start();
        }

        private void DoPowerUnleashedDamage()
        {
            DamageInteractableEnemeies(_powerUnleashedInteractables, _powerUnleashedDamage);
        }

        private void CancelPowerUnleashed()
        {
            if(!IsPowerUnleashing) {
                return;
            }

            _powerUnleashedDelayTimer.Stop();
            _powerUnleashedScaleTimer.Stop();
            _powerUnleashedVFX.Stop();

            _on_PowerUnleashed_Scale_Timer_timeout();
        }

        private void PowerUnleashed()
        {
            if(!CanPowerUnleashed) {
                return;
            }

            GD.Print($"[{Name}] Power unleashed!");
            Model.ChangeState("power_unleash");

            _powerUnleashedDelayTimer.Start();

            if(_shouldPowerUnleashedRoot) {
                Stop();
            }
        }

        private void Dash()
        {
            if(!CanDash) {
                if(IsPowerUnleashing && _dashCancelPowerUnleashed) {
                    CancelPowerUnleashed();
                    Dash();
                }
                return;
            }

            GD.Print($"[{Name}] Dash!");
            Model.ChangeState("dash");
            _dashAudioPlayer.Play();

            MaxSpeed *= _dashModifier;
            Velocity = Forward * MaxSpeed;

            _dashTimer.Start();
        }

        #region Signal Handlers

        private void _on_ClawAttack_Animation_Timer_timeout()
        {
            DoClawAttackDamage();

            _clawAttackVFX.Play("claw_attack");

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
            // TODO: this is dumb
            MaxSpeed /= _dashModifier;

            Model.ChangeState("Movement");

            _dashCooldown.Start();
        }

        private void _on_Death_Timer_timeout()
        {
            GD.Print($"[{Name}] died!");

            GameManager.Instance.GameOver(false);
        }

        #endregion

        #region Spawn

        public override void OnSpawn(SpawnPoint spawnPoint)
        {
            base.OnSpawn(spawnPoint);

            GameManager.Instance.Level.StageChangeEvent += StageChangeEventHandler;

            ResetHealth();
        }

        public override void OnReSpawn(SpawnPoint spawnPoint)
        {
            base.OnReSpawn(spawnPoint);

            ResetHealth();
        }

        public override void OnDeSpawn()
        {
            base.OnDeSpawn();

            if(GameManager.HasInstance && GameManager.Instance.Level != null) {
                GameManager.Instance.Level.StageChangeEvent -= StageChangeEventHandler;
            }
        }

        #endregion

        #region Event Handlers

        private void StageChangeEventHandler(object sender, EventArgs args)
        {
            ResetHealth();
        }

        #endregion
    }
}

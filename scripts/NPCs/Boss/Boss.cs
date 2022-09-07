using Godot;

using System.Linq;

using pdxpartyparrot.ssjAug2022.Managers;
using pdxpartyparrot.ssjAug2022.NPCs.AI;
using pdxpartyparrot.ssjAug2022.Player;
using pdxpartyparrot.ssjAug2022.World;

namespace pdxpartyparrot.ssjAug2022.NPCs.Boss
{
    public class Boss : Enemy
    {
        [Export]
        private float _trackingRange = 20.0f;

        public float TrackingRangeSquared => _trackingRange * _trackingRange;

        #region Attack

        [Export]
        private float _attackRange = 4.0f;

        public float AttackRangeSquared => _attackRange * _attackRange;

        [Export]
        private int _attackDamage = 1;

        private Interactables.Interactables _attackInteractables;

        private VFX _attackVFX;

        // TODO: this would be better if it was driven by the animation
        private Timer _attackAnimationTimer;

        // TODO: this would be better if it was driven by the animation
        private Timer _attackDamageTimer;

        private Timer _attackCooldown;

        private AudioStreamPlayer _attackAudioPlayer;

        private AudioStreamPlayer _deathAudioPlayer;

        private bool CanAttack => !IsDead && !IsGlobalCooldown && _attackAnimationTimer.IsStopped() && _attackCooldown.IsStopped();

        #endregion

        #region Power Unleashed

        [Export]
        private bool _shouldPowerUnleashedRoot;

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

        // TODO: this would be better if it was driven by the animation
        private Timer _powerUnleashedDamageTimer;

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

        private BossStateMachine _stateMachine;

        public BossSteering Steering { get; private set; }

        private bool IsGlobalCooldown => !_attackAnimationTimer.IsStopped() || IsPowerUnleashing || IsDashing;

        private bool IsRooted => _shouldPowerUnleashedRoot && IsPowerUnleashing;

        #region Godot Lifecycle

        public override void _Ready()
        {
            base._Ready();

            _attackAnimationTimer = GetNode<Timer>("Timers/Attack Animation Timer");
            _attackDamageTimer = GetNode<Timer>("Timers/Attack Damage Timer");
            _attackCooldown = GetNode<Timer>("Timers/Attack Cooldown");
            _attackInteractables = Pivot.GetNode<Interactables.Interactables>("Attack Hitbox");
            _attackVFX = Pivot.GetNode<VFX>("Attack VFX");
            _attackAudioPlayer = GetNode<AudioStreamPlayer>("SFX/Attack");

            _powerUnleashedDelayTimer = GetNode<Timer>("Timers/PowerUnleashed Delay Timer");
            _powerUnleashedScaleTimer = GetNode<Timer>("Timers/PowerUnleashed Scale Timer");
            _powerUnleashedDamageTimer = GetNode<Timer>("Timers/PowerUnleashed Damage Timer");
            _powerUnleashedCooldown = GetNode<Timer>("Timers/PowerUnleashed Cooldown");
            _powerUnleashedInteractables = Pivot.GetNode<Interactables.Interactables>("PowerUnleashed Hitbox");
            _powerUnleashedVFX = Pivot.GetNode<VFX>("PowerUnleashed VFX");
            _powerUnleashedInitialScale = _powerUnleashedInteractables.Scale;
            _powerUnleashedMaxScale = _powerUnleashedInitialScale * _powerUnleashedScale;

            _dashTimer = GetNode<Timer>("Timers/Dash Timer");
            _dashCooldown = GetNode<Timer>("Timers/Dash Cooldown");
            _dashAudioPlayer = GetNode<AudioStreamPlayer>("SFX/Dash");

            _deathTimer = GetNode<Timer>("Timers/Death Timer");
            _deathAudioPlayer = GetNode<AudioStreamPlayer>("SFX/Death");

            Steering = GetNode<BossSteering>("Steering");

            _stateMachine = GetNode<BossStateMachine>("StateMachine");
            _stateMachine.SetGlobalState(new States.Global());
            _stateMachine.ChangeState(new States.Idle());
        }

        public override void _Process(float delta)
        {
            // TODO: we want to do this at a fixed rate
            // rather than every frame
            _stateMachine.Run();

            base._Process(delta);

            if(IsPowerUnleashScaling) {
                float pct = 1.0f - PowerUnleashedPercent;
                var scale = _powerUnleashedInitialScale + (_powerUnleashedMaxScale - _powerUnleashedInitialScale) * pct;
                scale.y = _powerUnleashedInitialScale.y;
                _powerUnleashedInteractables.Scale = scale;
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            if(!IsDead) {
                Steering.Update(delta);
            }

            base._PhysicsProcess(delta);
        }

        #endregion

        private void ResetHealth()
        {
            CurrentHealth = MaxHealth;

            GameUIManager.Instance.HUD.UpdateBossHealth(1.0f);
        }

        protected override void OnDied()
        {
            base.OnDied();

            Model.ChangeState("death");

            _stateMachine.ChangeState(new States.Dead());

            _deathAudioPlayer.Play();

            // TODO: this should despawn but not destroy (do not decrease enemy count)
            _deathTimer.Start();
        }

        public override void Damage(int amount)
        {
            if(IsDead) {
                return;
            }

            base.Damage(amount);

            GameUIManager.Instance.HUD.UpdateBossHealth(MaxHealth > 0 ? CurrentHealth / (float)MaxHealth : 0.0f);
        }

        private void DamageInteractablePlayers(Interactables.Interactables interactables, int damage)
        {
            var players = interactables.GetInteractables<Vampire>();

            // copy because we're going to modify the underlying collection
            var vampires = new Vampire[players.Count];
            for(int idx = 0; idx < players.Count; ++idx) {
                vampires[idx] = (Vampire)players.ElementAt(idx);
            }

            foreach(var vampire in vampires) {
                vampire.Damage(damage);
            }
        }

        private void DoAttackDamage()
        {
            DamageInteractablePlayers(_attackInteractables, _attackDamage);
        }

        public bool Attack()
        {
            if(!CanAttack) {
                return false;
            }

            GD.Print($"[{Id}] Attack!");
            Model.TriggerOneShot("parameters/attack_movement/AttackTrigger/active");
            _attackAudioPlayer.Play();

            _attackAnimationTimer.Start();
            _attackDamageTimer.Start();

            return true;
        }

        private void DoPowerUnleashedDamage()
        {
            GD.Print($"[{Id}] Power unleashed damage!");

            DamageInteractablePlayers(_powerUnleashedInteractables, _powerUnleashedDamage);

            _powerUnleashedDamageTimer.Start();
        }

        public bool PowerUnleashed()
        {
            if(!CanPowerUnleashed) {
                return false;
            }

            GD.Print($"[{Id}] Power unleashed!");
            Model.ChangeState("power_unleash");

            _powerUnleashedDelayTimer.Start();

            if(_shouldPowerUnleashedRoot) {
                Stop();
            }

            return true;
        }

        public bool Dash()
        {
            if(!CanDash) {
                return false;
            }

            GD.Print($"[{Id}] Dash!");
            Model.ChangeState("dash");
            _dashAudioPlayer.Play();

            MaxSpeed *= _dashModifier;
            Velocity = Forward * MaxSpeed;

            _dashTimer.Start();

            return true;
        }

        protected override void UpdateVelocity(Vector3 velocity)
        {
            if(!IsDead && !IsDashing && !IsRooted) {
                base.UpdateVelocity(velocity);
            }
        }

        public override bool HandleMessage(Telegram message)
        {
            if(base.HandleMessage(message)) {
                return true;
            }

            return _stateMachine.HandleMessage(message);
        }

        #region Spawn

        public override void OnSpawn(SpawnPoint spawnPoint)
        {
            base.OnSpawn(spawnPoint);

            ResetHealth();
        }

        #endregion

        #region Signal Handlers

        private void _on_Attack_Animation_Timer_timeout()
        {
            _attackCooldown.Start();
        }

        private void _on_Attack_Damage_Timer_timeout()
        {
            DoAttackDamage();

            //_attackVFX.Play("attack");
        }

        private void _on_PowerUnleashed_Delay_Timer_timeout()
        {
            _powerUnleashedVFX.Play("power_unleashed");

            _powerUnleashedScaleTimer.Start();

            DoPowerUnleashedDamage();
        }

        private void _on_PowerUnleashed_Scale_Timer_timeout()
        {
            _powerUnleashedInteractables.Scale = _powerUnleashedInitialScale;

            _powerUnleashedVFX.Stop();
            _powerUnleashedDamageTimer.Stop();

            _powerUnleashedCooldown.Start();
        }

        private void _on_PowerUnleashed_Damage_Timer_timeout()
        {
            if(IsPowerUnleashScaling) {
                DoPowerUnleashedDamage();
            }
        }

        private void _on_Dash_Timer_timeout()
        {
            // TODO: this is dumb
            MaxSpeed /= _dashModifier;

            Model.ChangeState("Movement");

            _dashCooldown.Start();
        }

        private void _on_DetectionBox_area_entered(Area other)
        {
            if(!(other.Owner is Vampire vampire)) {
                return;
            }

            // TODO: it would be better if this was done
            // in the Global state so the AI is contained
            _stateMachine.ChangeState(new States.ChasePlayer {
                Target = vampire,
            });
        }

        private void _on_Death_Timer_timeout()
        {
            GD.Print($"[{Id}] died!");

            // TODO: this should destroy the NPC (decrease enemy count)
            NPCManager.Instance.DeSpawnNPC(this, true);

            GameManager.Instance.Level.EnemyDefeated();
        }

        #endregion
    }
}

using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;

using pdxpartyparrot.ssjAug2022.Interactables;
using pdxpartyparrot.ssjAug2022.Managers;
using pdxpartyparrot.ssjAug2022.Player;

namespace pdxpartyparrot.ssjAug2022.NPCs
{
    public class Human : SimpleNPC, IInteractable
    {
        [Export]
        private int _maxHealth = 1;

        private int _currentHealth;

        public bool IsDead => _currentHealth <= 0;

        [Export]
        private float _wanderSpeed = 5.0f;

        public float WanderSpeed => _wanderSpeed;

        [Export]
        private float _idleLeashRange = 10.0f;

        public float IdleLeashRangeSquared => _idleLeashRange * _idleLeashRange;

        [Export]
        private float _trackingRange = 20.0f;

        public float TrackingRangeSquared => _trackingRange * _trackingRange;

        [Export]
        private float _attackRange = 4.0f;

        public float AttackRangeSquared => _attackRange * _attackRange;

        #region Attack

        [Export]
        private int _attackDamage = 1;

        private Interactables.Interactables _attackInteractables;

        // TODO: this would be better if it was driven by the animation
        private Timer _attackAnimationTimer;

        private Timer _attackCooldown;

        private AudioStreamPlayer _attackAudioPlayer;

        #endregion

        public Vector3 HomeTranslation { get; private set; }

        private HumanStateMachine _stateMachine;

        public HumanSteering Steering { get; private set; }

        public bool CanInteract => !IsDead;

        public Type InteractableType => GetType();

        #region Godot Lifecycle

        public override void _Ready()
        {
            base._Ready();

            _currentHealth = _maxHealth;
            HomeTranslation = Translation;

            _attackAnimationTimer = GetNode<Timer>("Timers/Attack Animation Timer");
            _attackCooldown = GetNode<Timer>("Timers/Attack Cooldown");
            _attackInteractables = Pivot.GetNode<Interactables.Interactables>("Attack Hitbox");
            _attackAudioPlayer = GetNode<AudioStreamPlayer>("SFX/Attack");

            Steering = GetNode<HumanSteering>("Steering");

            _stateMachine = GetNode<HumanStateMachine>("StateMachine");
            _stateMachine.SetGlobalState(new States.Global());
            _stateMachine.ChangeState(new States.Idle());
        }

        public override void _Process(float delta)
        {
            // TODO: we want to do this at a fixed rate
            // rather than every frame
            _stateMachine.Run();

            base._Process(delta);
        }

        public override void _PhysicsProcess(float delta)
        {
            if(IsInputAllowed) {
                Steering.Update(delta);
            }

            base._PhysicsProcess(delta);
        }

        #endregion

        public async Task DamageAsync(int amount)
        {
            _currentHealth = Mathf.Max(_currentHealth - amount, 0);

            if(IsDead) {
                NPCManager.Instance.DeSpawnNPC(this, true);

                await GameManager.Instance.EnemyDefeatedAsync().ConfigureAwait(false);
            }
        }

        private async Task DamageInteractablePlayersAsync(Interactables.Interactables interactables, int damage)
        {
            var players = interactables.GetInteractables<Vampire>();

            // copy because we're going to modify the underlying collection
            var vampires = new Vampire[players.Count];
            for(int idx = 0; idx < players.Count; ++idx) {
                vampires[idx] = (Vampire)players.ElementAt(idx);
            }

            foreach(var vampire in vampires) {
                await vampire.DamageAsync(damage).ConfigureAwait(false);
            }
        }

        private async Task DoAttackDamageAsync()
        {
            await DamageInteractablePlayersAsync(_attackInteractables, _attackDamage).ConfigureAwait(false);
        }

        public void Attack(Vampire target)
        {
            if(!_attackAnimationTimer.IsStopped() || !_attackCooldown.IsStopped()) {
                return;
            }

            GD.Print($"[{Id}] Attack!");
            Model.TriggerOneShot("parameters/Claw_AttackTrigger/active");
            _attackAudioPlayer.Play();

            _attackAnimationTimer.Start();
        }

        #region Signal Handlers

        private async void _on_Attack_Animation_Timer_timeout()
        {
            await DoAttackDamageAsync().ConfigureAwait(false);

            _attackCooldown.Start();
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

        #endregion
    }
}

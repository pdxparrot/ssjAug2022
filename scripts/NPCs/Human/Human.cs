using Godot;

using System.Linq;

using pdxpartyparrot.ssjAug2022.Managers;
using pdxpartyparrot.ssjAug2022.Player;

namespace pdxpartyparrot.ssjAug2022.NPCs.Human
{
    public class Human : Enemy
    {
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

        // TODO: this would be better if it was driven by the animation
        private Timer _attackDamageTimer;

        private Timer _attackCooldown;

        private AudioStreamPlayer _attackAudioPlayer;

        private AudioStreamPlayer _deathAudioPlayer;

        #endregion

        private Timer _deathTimer;

        public Vector3 HomeTranslation { get; private set; }

        private HumanStateMachine _stateMachine;

        public HumanSteering Steering { get; private set; }

        private bool IsGlobalCooldown => !_attackAnimationTimer.IsStopped();

        #region Godot Lifecycle

        public override void _Ready()
        {
            base._Ready();

            HomeTranslation = Translation;

            _attackAnimationTimer = GetNode<Timer>("Timers/Attack Animation Timer");
            _attackDamageTimer = GetNode<Timer>("Timers/Attack Damage Timer");
            _attackCooldown = GetNode<Timer>("Timers/Attack Cooldown");
            _attackInteractables = Pivot.GetNode<Interactables.Interactables>("Attack Hitbox");
            _attackAudioPlayer = GetNode<AudioStreamPlayer>("SFX/Attack");

            _deathTimer = GetNode<Timer>("Timers/Death Timer");
            _deathAudioPlayer = GetNode<AudioStreamPlayer>("SFX/Death");

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
            if(!IsDead) {
                Steering.Update(delta);
            }

            base._PhysicsProcess(delta);
        }

        #endregion

        protected override void OnDied()
        {
            base.OnDied();

            Model.ChangeState("death");

            _stateMachine.ChangeState(new States.Dead());

            _deathAudioPlayer.Play();

            // TODO: this should despawn but not destroy (do not decrease enemy count)
            _deathTimer.Start();
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

        public void Attack()
        {
            if(IsDead || IsGlobalCooldown || !_attackCooldown.IsStopped()) {
                return;
            }

            GD.Print($"[{Id}] Attack!");
            Model.TriggerOneShot("parameters/BlendTree/Claw_Attack_Trigger/active");
            _attackAudioPlayer.Play();

            _attackAnimationTimer.Start();
            _attackDamageTimer.Start();
        }

        protected override void UpdateVelocity(Vector3 velocity)
        {
            if(!IsDead) {
                base.UpdateVelocity(velocity);
            }
        }

        #region Signal Handlers

        private void _on_Attack_Animation_Timer_timeout()
        {
            _attackCooldown.Start();
        }

        private void _on_Attack_Damage_Timer_timeout()
        {
            DoAttackDamage();
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

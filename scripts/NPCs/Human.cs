using Godot;

using System;
using System.Threading.Tasks;

using pdxpartyparrot.ssjAug2022.Interactables;
using pdxpartyparrot.ssjAug2022.Managers;

namespace pdxpartyparrot.ssjAug2022.NPCs
{
    public class Human : SimpleNPC, IInteractable
    {
        [Export]
        private int _maxHealth = 1;

        private int _currentHealth;

        public bool IsDead => _currentHealth <= 0;

        [Export]
        private float _idleLeashRange = 10.0f;

        public float IdleLeashRange => _idleLeashRange;

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

                await GameManager.Instance.EnemyDefeatedAsync();
            }
        }
    }
}

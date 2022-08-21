using Godot;

using System.Threading.Tasks;

using pdxpartyparrot.ssjAug2022.Managers;
using pdxpartyparrot.ssjAug2022.NPCs.AI;

namespace pdxpartyparrot.ssjAug2022.NPCs
{
    public class Human : SimpleNPC
    {
        [Export]
        private int _maxHealth = 1;

        private int _currentHealth;

        public bool IsDead => _currentHealth <= 0;

        private StateMachine<Human> _stateMachine;

        private Steering _steering = new Steering();

        #region Godot Lifecycle

        public override void _Ready()
        {
            base._Ready();

            _currentHealth = _maxHealth;

            _stateMachine = new StateMachine<Human>(this, new States.Idle(), new States.Global());
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
                Heading = _steering.Calculate();
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

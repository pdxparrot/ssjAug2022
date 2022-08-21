using pdxpartyparrot.ssjAug2022.Collections;
using pdxpartyparrot.ssjAug2022.Managers;
using pdxpartyparrot.ssjAug2022.NPCs.AI;

namespace pdxpartyparrot.ssjAug2022.NPCs.States
{
    public struct ChasePlayer : IState<Human>
    {
        public void Enter(Human owner, StateMachine<Human> stateMachine)
        {
            //PlayerManager.Instance.Players.Values.NearestManhattan(owner.GlobalTranslation, out float distance);
            //owner.SetTarget(...);
        }

        public void Exit(Human owner, StateMachine<Human> stateMachine)
        {
        }

        public void Execute(Human owner, StateMachine<Human> stateMachine)
        {
            // TODO: if target out of range, go back to idle
        }
    }
}

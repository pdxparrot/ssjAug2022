using pdxpartyparrot.ssjAug2022.Collections;
using pdxpartyparrot.ssjAug2022.Managers;
using pdxpartyparrot.ssjAug2022.NPCs.AI;

namespace pdxpartyparrot.ssjAug2022.NPCs.States
{
    public struct Global : IState<Human>
    {
        public void Enter(Human owner, StateMachine<Human> stateMachine)
        {
        }

        public void Exit(Human owner, StateMachine<Human> stateMachine)
        {
        }

        public void Execute(Human owner, StateMachine<Human> stateMachine)
        {
            //PlayerManager.Instance.Players.Values.NearestManhattan(owner.GlobalTranslation, out float distance);
            // TODO: if player found in range, change to ChasePlayer state
        }
    }
}

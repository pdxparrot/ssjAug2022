using pdxpartyparrot.ssjAug2022.NPCs.AI;

namespace pdxpartyparrot.ssjAug2022.NPCs.States
{
    public struct Dead : IState<Human>
    {
        public void Enter(Human owner, StateMachine<Human> stateMachine)
        {
        }

        public void Exit(Human owner, StateMachine<Human> stateMachine)
        {
        }

        public void Execute(Human owner, StateMachine<Human> stateMachine)
        {
        }

        public bool OnMessage(Human owner, StateMachine<Human> stateMachine, Telegram message)
        {
            return false;
        }
    }
}

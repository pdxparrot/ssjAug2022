using pdxpartyparrot.ssjAug2022.NPCs.AI;

namespace pdxpartyparrot.ssjAug2022.NPCs.Boss.States
{
    public struct Dead : IState<Boss>
    {
        public void Enter(Boss owner, StateMachine<Boss> stateMachine)
        {
        }

        public void Exit(Boss owner, StateMachine<Boss> stateMachine)
        {
        }

        public void Execute(Boss owner, StateMachine<Boss> stateMachine)
        {
        }

        public bool OnMessage(Boss owner, StateMachine<Boss> stateMachine, Telegram message)
        {
            return false;
        }
    }
}

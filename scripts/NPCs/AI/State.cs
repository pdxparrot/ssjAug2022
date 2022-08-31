namespace pdxpartyparrot.ssjAug2022.NPCs.AI
{
    public interface IState<T> where T : SimpleNPC
    {
        void Enter(T owner, StateMachine<T> stateMachine);

        void Exit(T owner, StateMachine<T> stateMachine);

        void Execute(T owner, StateMachine<T> stateMachine);

        bool OnMessage(T owner, StateMachine<T> stateMachine, Telegram message);
    }
}

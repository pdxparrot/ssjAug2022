namespace pdxpartyparrot.ssjAug2022.NPCs.AI
{
    public abstract class State<T> where T : SimpleNPC
    {
        public abstract void Enter(T owner, StateMachine<T> stateMachine);

        public abstract void Exit(T owner, StateMachine<T> stateMachine);

        public abstract void Execute(T owner, StateMachine<T> stateMachine);
    }
}

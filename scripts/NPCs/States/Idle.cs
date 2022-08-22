using pdxpartyparrot.ssjAug2022.NPCs.AI;

namespace pdxpartyparrot.ssjAug2022.NPCs.States
{
    public struct Idle : IState<Human>
    {
        public void Enter(Human owner, StateMachine<Human> stateMachine)
        {
            owner.MaxSpeed = owner.WalkSpeed;

            owner.Steering.WanderOn();
        }

        public void Exit(Human owner, StateMachine<Human> stateMachine)
        {
            owner.Steering.WanderOff();
        }

        public void Execute(Human owner, StateMachine<Human> stateMachine)
        {
        }
    }
}

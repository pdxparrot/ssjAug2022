using pdxpartyparrot.ssjAug2022.NPCs.AI;

namespace pdxpartyparrot.ssjAug2022.NPCs.States
{
    public struct Idle : IState<Human>
    {
        public void Enter(Human owner, StateMachine<Human> stateMachine)
        {
            owner.Steering.WanderOn(new HumanSteering.WanderParams {
                radius = 1.0f,
                distance = 10.0f,
                jitter = 80.0f,
            });
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

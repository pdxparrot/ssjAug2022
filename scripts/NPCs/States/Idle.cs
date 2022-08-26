using pdxpartyparrot.ssjAug2022.NPCs.AI;

namespace pdxpartyparrot.ssjAug2022.NPCs.States
{
    public struct Idle : IState<Human>
    {
        public void Enter(Human owner, StateMachine<Human> stateMachine)
        {
            owner.Steering.WanderOn(new HumanSteering.WanderParams {
                radius = 5.0f,
                distance = 10.0f,
                jitter = 50.0f,
                maxSpeed = owner.MaxSpeed,
            });
        }

        public void Exit(Human owner, StateMachine<Human> stateMachine)
        {
            owner.Steering.WanderOff();
        }

        public void Execute(Human owner, StateMachine<Human> stateMachine)
        {
            if(owner.Translation.DistanceSquaredTo(owner.HomeTranslation) > owner.IdleLeashRangeSquared) {
                stateMachine.ChangeState(new ReturnHome());
            }
        }
    }
}

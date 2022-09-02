using pdxpartyparrot.ssjAug2022.NPCs.AI;

namespace pdxpartyparrot.ssjAug2022.NPCs.Human.States
{
    public struct ReturnHome : IState<Human>
    {
        public void Enter(Human owner, StateMachine<Human> stateMachine)
        {
            owner.Steering.SeekOn(new HumanSteering.SeekParams {
                target = owner.HomeTranslation,
                maxSpeed = owner.WanderSpeed,
            });
        }

        public void Exit(Human owner, StateMachine<Human> stateMachine)
        {
            owner.Steering.SeekOff();
        }

        public void Execute(Human owner, StateMachine<Human> stateMachine)
        {
            float homeDistance = owner.Translation.DistanceSquaredTo(owner.HomeTranslation);
            if(homeDistance <= 1.0f) {
                stateMachine.ChangeState(new Idle());
            }
        }

        public bool OnMessage(Human owner, StateMachine<Human> stateMachine, Telegram message)
        {
            return false;
        }
    }
}

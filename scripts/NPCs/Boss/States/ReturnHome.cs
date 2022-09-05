using pdxpartyparrot.ssjAug2022.NPCs.AI;

namespace pdxpartyparrot.ssjAug2022.NPCs.Boss.States
{
    public struct ReturnHome : IState<Boss>
    {
        public void Enter(Boss owner, StateMachine<Boss> stateMachine)
        {
            owner.Steering.SeekOn(new BossSteering.SeekParams {
                target = owner.HomeTranslation,
                maxSpeed = owner.WanderSpeed,
            });
        }

        public void Exit(Boss owner, StateMachine<Boss> stateMachine)
        {
            owner.Steering.SeekOff();
        }

        public void Execute(Boss owner, StateMachine<Boss> stateMachine)
        {
            // idle when we get home
            float homeDistance = owner.GlobalTranslation.DistanceSquaredTo(owner.HomeTranslation);
            if(homeDistance <= 1.0f) {
                stateMachine.ChangeState(new Idle());
            }
        }

        public bool OnMessage(Boss owner, StateMachine<Boss> stateMachine, Telegram message)
        {
            return false;
        }
    }
}

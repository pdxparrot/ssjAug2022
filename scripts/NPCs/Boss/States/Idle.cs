using pdxpartyparrot.ssjAug2022.NPCs.AI;

namespace pdxpartyparrot.ssjAug2022.NPCs.Boss.States
{
    public struct Idle : IState<Boss>
    {
        public void Enter(Boss owner, StateMachine<Boss> stateMachine)
        {
            owner.Steering.WanderOn(new BossSteering.WanderParams {
                // TODO: make these configurable
                radius = 5.0f,
                distance = 10.0f,
                jitter = 50.0f,
                maxSpeed = owner.WanderSpeed,
            });
        }

        public void Exit(Boss owner, StateMachine<Boss> stateMachine)
        {
            owner.Steering.WanderOff();
        }

        public void Execute(Boss owner, StateMachine<Boss> stateMachine)
        {
            // leash if we go too far
            float homeDistance = owner.Translation.DistanceSquaredTo(owner.HomeTranslation);
            if(homeDistance > owner.IdleLeashRangeSquared) {
                stateMachine.ChangeState(new ReturnHome());
            }
        }

        public bool OnMessage(Boss owner, StateMachine<Boss> stateMachine, Telegram message)
        {
            return false;
        }
    }
}

using Godot;

using pdxpartyparrot.ssjAug2022.NPCs.AI;
using pdxpartyparrot.ssjAug2022.Player;

namespace pdxpartyparrot.ssjAug2022.NPCs.Boss.States
{
    public struct ChasePlayer : IState<Boss>
    {
        public Vampire Target { get; set; }

        public void Enter(Boss owner, StateMachine<Boss> stateMachine)
        {
            //GD.Print($"[{owner.Id} chasing vampire {Target.Name}");

            owner.Steering.PursuitOn(new BossSteering.PursuitParams {
                target = Target,
                maxSpeed = owner.MaxSpeed,
            });
        }

        public void Exit(Boss owner, StateMachine<Boss> stateMachine)
        {
            owner.Steering.PursuitOff();
        }

        public void Execute(Boss owner, StateMachine<Boss> stateMachine)
        {
            if(Target.IsDead) {
                //GD.Print($"[{owner.Id} my target died");
                stateMachine.ChangeState(new ReturnHome());
                return;
            }

            float targetDistance = owner.GlobalTranslation.DistanceSquaredTo(Target.GlobalTranslation);

            if(targetDistance > owner.TrackingRangeSquared) {
                //GD.Print($"[{owner.Id} lost my target");
                stateMachine.ChangeState(new ReturnHome());
                return;
            }

            if(targetDistance <= owner.AttackRangeSquared) {
                stateMachine.ChangeState(new AttackPlayer {
                    Target = Target,
                });
                return;
            }
        }

        public bool OnMessage(Boss owner, StateMachine<Boss> stateMachine, Telegram message)
        {
            return false;
        }
    }
}

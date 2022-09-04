using Godot;

using pdxpartyparrot.ssjAug2022.NPCs.AI;
using pdxpartyparrot.ssjAug2022.Player;

namespace pdxpartyparrot.ssjAug2022.NPCs.Boss.States
{
    public struct AttackPlayer : IState<Boss>
    {
        public Vampire Target { get; set; }

        public void Enter(Boss owner, StateMachine<Boss> stateMachine)
        {
            //GD.Print($"[{owner.Id} attacking vampire {Target.Name}");

            owner.Stop();

            /*owner.Steering.PursuitOn(new BossSteering.PursuitParams {
                target = Target,
                maxSpeed = owner.MaxSpeed,
            });*/
        }

        public void Exit(Boss owner, StateMachine<Boss> stateMachine)
        {
            //owner.Steering.PursuitOff();
        }

        public void Execute(Boss owner, StateMachine<Boss> stateMachine)
        {
            if(Target.IsDead) {
                //GD.Print($"[{owner.Id} my target died");
                stateMachine.ChangeState(new ReturnHome());
                return;
            }

            float targetDistance = owner.GlobalTranslation.DistanceSquaredTo(Target.GlobalTranslation);
            if(targetDistance > owner.AttackRangeSquared) {
                //GD.Print($"[{owner.Id} too far from target to attack");
                stateMachine.ChangeState(new ChasePlayer {
                    Target = Target,
                });
                return;
            }

            owner.Attack();
        }

        public bool OnMessage(Boss owner, StateMachine<Boss> stateMachine, Telegram message)
        {
            return false;
        }
    }
}

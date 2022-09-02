using Godot;

using pdxpartyparrot.ssjAug2022.NPCs.AI;
using pdxpartyparrot.ssjAug2022.Player;

namespace pdxpartyparrot.ssjAug2022.NPCs.Human.States
{
    public struct ChasePlayer : IState<Human>
    {
        public Vampire Target { get; set; }

        public void Enter(Human owner, StateMachine<Human> stateMachine)
        {
            //GD.Print($"[{owner.Id} chasing vampire {Target.Name}");

            owner.Steering.PursuitOn(new HumanSteering.PursuitParams {
                target = Target,
                maxSpeed = owner.MaxSpeed,
            });
        }

        public void Exit(Human owner, StateMachine<Human> stateMachine)
        {
            owner.Steering.PursuitOff();
        }

        public void Execute(Human owner, StateMachine<Human> stateMachine)
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

        public bool OnMessage(Human owner, StateMachine<Human> stateMachine, Telegram message)
        {
            return false;
        }
    }
}

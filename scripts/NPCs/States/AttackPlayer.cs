using Godot;

using pdxpartyparrot.ssjAug2022.NPCs.AI;
using pdxpartyparrot.ssjAug2022.Player;

namespace pdxpartyparrot.ssjAug2022.NPCs.States
{
    public struct AttackPlayer : IState<Human>
    {
        public Vampire Target { get; set; }

        public void Enter(Human owner, StateMachine<Human> stateMachine)
        {
            //GD.Print($"[{owner.Id} attacking vampire {Target.Name}");

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
            if(owner.GlobalTranslation.DistanceSquaredTo(Target.GlobalTranslation) > owner.AttackRangeSquared) {
                //GD.Print($"[{owner.Id} too far from target to attack");
                stateMachine.ChangeState(new ChasePlayer {
                    Target = Target,
                });
                return;
            }

            owner.Attack(Target);
        }
    }
}

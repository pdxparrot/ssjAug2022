using Godot;

using pdxpartyparrot.ssjAug2022.NPCs.AI;
using pdxpartyparrot.ssjAug2022.Player;

namespace pdxpartyparrot.ssjAug2022.NPCs.Human.States
{
    public struct AttackPlayer : IState<Human>
    {
        public Vampire Target { get; set; }

        public void Enter(Human owner, StateMachine<Human> stateMachine)
        {
            //GD.Print($"[{owner.Id} attacking vampire {Target.Name}");

            owner.Stop();

            /*owner.Steering.PursuitOn(new HumanSteering.PursuitParams {
                target = Target,
                maxSpeed = owner.MaxSpeed,
            });*/
        }

        public void Exit(Human owner, StateMachine<Human> stateMachine)
        {
            //owner.Steering.PursuitOff();
        }

        public void Execute(Human owner, StateMachine<Human> stateMachine)
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

            // TODO: we should turn towards the player by max turn rate
            // and only attack if the player is in our arc after doing so
            owner.Pivot.LookAt(Target.GlobalTranslation, Vector3.Up);

            owner.Attack();
        }

        public bool OnMessage(Human owner, StateMachine<Human> stateMachine, Telegram message)
        {
            return false;
        }
    }
}

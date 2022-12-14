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

            // TODO: if the player is near us but not in front of us, AOE
            // otherwise do the frontal attack

            var toTarget = (Target.GlobalTranslation - owner.GlobalTranslation).Normalized();
            float dot = owner.Forward.Dot(toTarget);
            if(dot > 0.0f) {
                if(!owner.Attack()) {
                    // TODO: we should turn towards the player by max turn rate
                    // and only attack if the player is in our arc after doing so
                    //owner.Pivot.LookAt(Target.GlobalTranslation, Vector3.Up);
                }
            } else {
                if(!owner.PowerUnleashed()) {
                    // TODO: we should turn towards the player by max turn rate
                    // and only attack if the player is in our arc after doing so
                    //owner.Pivot.LookAt(Target.GlobalTranslation, Vector3.Up);
                }
            }
        }

        public bool OnMessage(Boss owner, StateMachine<Boss> stateMachine, Telegram message)
        {
            return false;
        }
    }
}

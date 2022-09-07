using Godot;

using pdxpartyparrot.ssjAug2022.NPCs.AI;
using pdxpartyparrot.ssjAug2022.Player;

namespace pdxpartyparrot.ssjAug2022.NPCs.Human.States
{
    public struct Global : IState<Human>
    {
        public void Enter(Human owner, StateMachine<Human> stateMachine)
        {
        }

        public void Exit(Human owner, StateMachine<Human> stateMachine)
        {
        }

        public void Execute(Human owner, StateMachine<Human> stateMachine)
        {
        }

        public bool OnMessage(Human owner, StateMachine<Human> stateMachine, Telegram message)
        {
            if(owner.IsDead) {
                return false;
            }

            if(message.Message == (int)Messages.VampireSpotted) {
                if(stateMachine.IsInState(typeof(ChasePlayer)) || stateMachine.IsInState(typeof(AttackPlayer))) {
                    return false;
                }

                float distance = owner.GlobalTranslation.DistanceSquaredTo(message.Sender.GlobalTranslation);
                if(distance > owner.AlarmRangeSquared) {
                    return false;
                }

                GD.Print($"[{owner.Id}] Vampire alarm!");
                stateMachine.ChangeState(new ChasePlayer {
                    Target = (Vampire)message.ExtraInfo,
                    Alarm = false,
                });

                return false;
            }

            return false;
        }
    }
}

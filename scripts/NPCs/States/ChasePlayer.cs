using pdxpartyparrot.ssjAug2022.NPCs.AI;
using pdxpartyparrot.ssjAug2022.Player;

namespace pdxpartyparrot.ssjAug2022.NPCs.States
{
    public struct ChasePlayer : IState<Human>
    {
        public void Enter(Human owner, StateMachine<Human> stateMachine)
        {
            owner.Steering.PursuitOn(new HumanSteering.PursuitParams {
                target = owner.Target,
                maxSpeed = owner.MaxSpeed,
            });
        }

        public void Exit(Human owner, StateMachine<Human> stateMachine)
        {
        }

        public void Execute(Human owner, StateMachine<Human> stateMachine)
        {
            // TODO: if target out of range, go back to idle

            // TODO: if the player is in range, try to attack it
            // (should this be a new state?)
        }
    }
}

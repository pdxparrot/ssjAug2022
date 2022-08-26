using Godot;

using pdxpartyparrot.ssjAug2022.NPCs.AI;
using pdxpartyparrot.ssjAug2022.Player;

namespace pdxpartyparrot.ssjAug2022.NPCs.States
{
    public struct ChasePlayer : IState<Human>
    {
        public Vampire Target { get; set; }

        public void Enter(Human owner, StateMachine<Human> stateMachine)
        {
            //GD.Print($"[{owner.Id} chasing vampire {Target.ClientId}");

            owner.Steering.PursuitOn(new HumanSteering.PursuitParams {
                target = Target,
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

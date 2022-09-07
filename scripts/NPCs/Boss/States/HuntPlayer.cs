using Godot;

using pdxpartyparrot.ssjAug2022.Collections;
using pdxpartyparrot.ssjAug2022.Managers;
using pdxpartyparrot.ssjAug2022.NPCs.AI;
using pdxpartyparrot.ssjAug2022.Util;

namespace pdxpartyparrot.ssjAug2022.NPCs.Boss.States
{
    public struct HuntPlayer : IState<Boss>
    {
        private Vector3 _target;

        private ulong _lastTargetUpdate;

        public void Enter(Boss owner, StateMachine<Boss> stateMachine)
        {
            var target = PlayerManager.Instance.Players.GetRandomEntry();
            _target = target.Value.GlobalTranslation;

            // fuzz the target a little bit so we don't beeline the player
            var offset = new Vector3(
                            PartyParrotManager.Instance.Random.NextSingle(-5.0f, 5.0f),
                            0.0f,
                            PartyParrotManager.Instance.Random.NextSingle(-5.0f, 5.0f)
                        );
            _target += offset;

            GD.Print($"[{owner.Id}] hunting player {target.Value.Name} at {_target}");

            _lastTargetUpdate = Time.GetTicksMsec();

            owner.Steering.SeekOn(new BossSteering.SeekParams {
                target = _target,
                maxSpeed = owner.WanderSpeed,
            });
        }

        public void Exit(Boss owner, StateMachine<Boss> stateMachine)
        {
            owner.Steering.SeekOff();
        }

        public void Execute(Boss owner, StateMachine<Boss> stateMachine)
        {
            // keep hunting
            float targetDistance = owner.GlobalTranslation.DistanceSquaredTo(_target);
            if(targetDistance <= 1.0f || Time.GetTicksMsec() - _lastTargetUpdate > 5000) {
                stateMachine.ReEnterState();
            }
        }

        public bool OnMessage(Boss owner, StateMachine<Boss> stateMachine, Telegram message)
        {
            return false;
        }
    }
}

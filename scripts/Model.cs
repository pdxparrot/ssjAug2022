using Godot;

namespace pdxpartyparrot.ssjAug2022
{
    public class Model : Node
    {
        private AnimationNodeStateMachinePlayback _stateMachine;

        #region Godot Lifecycle

        public override void _Ready()
        {
            var animationTree = GetNode<AnimationTree>("AnimationTree");
            _stateMachine = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
            if(_stateMachine == null) {
                GD.Print("no state?");
            }
        }

        #endregion

        public void Travel(string toNode)
        {
            //_stateMachine.Travel(toNode);
        }
    }
}

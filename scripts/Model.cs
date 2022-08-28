using Godot;

using System;

namespace pdxpartyparrot.ssjAug2022
{
    public class Model : Node
    {
        [Export]
        private string _motionBlendPath;

        private AnimationTree _animationTree;

        private AnimationNodeStateMachinePlayback _animationStateMachine;


        #region Godot Lifecycle

        public override void _Ready()
        {
            _animationTree = GetNode<AnimationTree>("AnimationTree");
            _animationStateMachine = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");

        }

        #endregion

        public void UpdateMotionBlend(float amount)
        {
            _animationTree.Set(_motionBlendPath, Math.Abs(amount));
        }

        public void TriggerOneShot(string property)
        {
            _animationTree.Set(property, true);
        }

        public void Travel(string toNode)
        {
            if(_animationStateMachine != null) {
                _animationStateMachine.Travel(toNode);
            }
        }
    }
}

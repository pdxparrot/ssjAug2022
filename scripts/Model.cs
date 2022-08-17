using Godot;

using System;

namespace pdxpartyparrot.ssjAug2022
{
    public class Model : Node
    {
        private AnimationTree _animationTree;

        //private AnimationNodeStateMachinePlayback _stateMachine;

        #region Godot Lifecycle

        public override void _Ready()
        {
            _animationTree = GetNode<AnimationTree>("AnimationTree");
            //_stateMachine = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");
        }

        #endregion

        public void UpdateMotionBlend(float amount)
        {
            _animationTree.Set("parameters/Motion/blend_position", Math.Abs(amount));
        }

        public void TriggerOneShot(string property)
        {
            _animationTree.Set(property, true);
        }

        public void Travel(string toNode)
        {
            //_stateMachine.Travel(toNode);
        }
    }
}

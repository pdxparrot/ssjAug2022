using Godot;

using System;

namespace pdxpartyparrot.ssjAug2022
{
    public class Model : Node
    {
        private AnimationTree _animationTree;

        #region Godot Lifecycle

        public override void _Ready()
        {
            _animationTree = GetNode<AnimationTree>("AnimationTree");
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
    }
}

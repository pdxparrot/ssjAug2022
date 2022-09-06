using Godot;

namespace pdxpartyparrot.ssjAug2022
{
    public class VFX : Node
    {
        private AnimationPlayer _animationPlayer;

        #region Godot Lifecycle

        public override void _Ready()
        {
            _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        }

        #endregion

        public void Play(string animationName = "")
        {
            _animationPlayer.Play(animationName);
        }

        public void Stop()
        {
            _animationPlayer.Stop();
            _animationPlayer.Seek(0, true);
        }
    }
}

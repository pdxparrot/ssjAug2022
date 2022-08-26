using Godot;

namespace pdxpartyparrot.ssjAug2022.UI
{
    public class Button : TextureButton
    {
        // TODO: not sure what signal to use here
        private AudioStreamPlayer _hoverAudioStreamPlayer;

        private AudioStreamPlayer _pressedAudioStreamPlayer;

        #region Godot Lifecycle

        public override void _Ready()
        {
            _hoverAudioStreamPlayer = GetNode<AudioStreamPlayer>("Hover");
            _pressedAudioStreamPlayer = GetNode<AudioStreamPlayer>("Pressed");
        }

        #endregion

        #region Signal Handlers

        private void _on_Button_pressed()
        {
            _pressedAudioStreamPlayer.Play();
        }

        #endregion
    }
}

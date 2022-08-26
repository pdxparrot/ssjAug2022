using Godot;

using pdxpartyparrot.ssjAug2022.Managers;

namespace pdxpartyparrot.ssjAug2022.UI
{
    public class MainMenu : Control
    {
        // TODO: this belongs on the main menu state, not the UI
        private AudioStreamPlayer _musicPlayer;

        #region Godot Lifecycle

        public override void _Ready()
        {
            _musicPlayer = GetNode<AudioStreamPlayer>("Music");
            _musicPlayer.Play();
        }

        #endregion

        #region Signal Handlers

        private async void _on_Play_pressed()
        {
            await GameManager.Instance.StartGameAsync().ConfigureAwait(false);
        }

        #endregion
    }
}

using Godot;

using pdxpartyparrot.ssjAug2022.Managers;

namespace pdxpartyparrot.ssjAug2022.UI
{
    public class MainMenu : Control
    {
        // TODO: this belongs on the main menu state, not the UI
        [Export]
        private AudioStream _music;

        #region Godot Lifecycle

        public override void _Ready()
        {
        }

        public override void _EnterTree()
        {
            AudioManager.Instance.PlayMusic(_music);
        }

        public override void _ExitTree()
        {
            // TODO: this fires after the level starts playing its music
            //AudioManager.Instance.StopAllMusic();
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

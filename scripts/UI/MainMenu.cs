using Godot;

using pdxpartyparrot.ssjAug2022.Managers;

namespace pdxpartyparrot.ssjAug2022.UI
{
    public class MainMenu : Control
    {
        private Control _mainMenu;

        private Control _credits;

        // TODO: this belongs on the main menu state, not the UI
        private AudioStreamPlayer _musicPlayer;

        #region Godot Lifecycle

        public override void _Ready()
        {
            _mainMenu = GetNode<Control>("CanvasLayer/Main Menu");
            _mainMenu.Show();

            _credits = GetNode<Control>("CanvasLayer/Credits");
            _credits.Hide();

            _musicPlayer = GetNode<AudioStreamPlayer>("Music");
            _musicPlayer.Play();
        }

        #endregion

        #region Signal Handlers

        private async void _on_Play_pressed()
        {
            await GameManager.Instance.StartGameAsync().ConfigureAwait(false);
        }

        private async void _on_Credits_pressed()
        {
            _mainMenu.Hide();
            _credits.Show();
        }

        private async void _on_Credits_Back_pressed()
        {
            _mainMenu.Show();
            _credits.Hide();
        }

        #endregion
    }
}

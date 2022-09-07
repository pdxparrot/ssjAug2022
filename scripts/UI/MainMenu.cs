using Godot;

using pdxpartyparrot.ssjAug2022.Managers;

namespace pdxpartyparrot.ssjAug2022.UI
{
    public class MainMenu : Control
    {
        private Control _mainMenu;

        private Control _credits;

        private BaseButton _playButton;

        private BaseButton _creditsButton;

        private BaseButton _windowedButton;

        private BaseButton _fullscreenButton;

        private BaseButton _creditsBackButton;

        // TODO: this belongs on the main menu state, not the UI
        private AudioStreamPlayer _musicPlayer;

        #region Godot Lifecycle

        public override void _Ready()
        {
            var canvas = GetNode<CanvasLayer>("CanvasLayer");

            _mainMenu = canvas.GetNode<Control>("Main Menu");

            _playButton = _mainMenu.GetNode<BaseButton>("VBoxContainer/Play");
            _playButton.GrabFocus();

            _creditsButton = _mainMenu.GetNode<BaseButton>("VBoxContainer/Credits");

            _windowedButton = _mainMenu.GetNode<BaseButton>("VBoxContainer/Windowed");
            _fullscreenButton = _mainMenu.GetNode<BaseButton>("VBoxContainer/Fullscreen");
            UpdateFullscreenButtons();

            _mainMenu.Show();

            _credits = canvas.GetNode<Control>("Credits");
            _creditsBackButton = canvas.GetNode<BaseButton>("Credits/Back");
            _credits.Hide();

            _musicPlayer = GetNode<AudioStreamPlayer>("Music");
            _musicPlayer.Play();
        }

        public override void _EnterTree()
        {
            if(_playButton != null) {
                _playButton.GrabFocus();
            }
        }

        #endregion

        private void UpdateFullscreenButtons()
        {
            _windowedButton.Visible = PartyParrotManager.Instance.IsFullscreen;
            _fullscreenButton.Visible = !PartyParrotManager.Instance.IsFullscreen;
        }

        #region Signal Handlers

        private async void _on_Play_pressed()
        {
            await GameManager.Instance.StartGameAsync().ConfigureAwait(false);
        }

        private void _on_Windowed_pressed()
        {
            PartyParrotManager.Instance.IsFullscreen = false;

            UpdateFullscreenButtons();

            _fullscreenButton.GrabFocus();
        }

        private void _on_Fullscreen_pressed()
        {
            PartyParrotManager.Instance.IsFullscreen = true;

            UpdateFullscreenButtons();

            _windowedButton.GrabFocus();
        }

        private void _on_Credits_pressed()
        {
            _mainMenu.Hide();
            _credits.Show();

            _creditsBackButton.GrabFocus();
        }

        private void _on_Credits_Back_pressed()
        {
            _mainMenu.Show();
            _credits.Hide();

            _creditsButton.GrabFocus();
        }

        private void _on_Quit_pressed()
        {
            PartyParrotManager.Instance.SafeQuit();
        }

        #endregion
    }
}

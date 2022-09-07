using Godot;

using pdxpartyparrot.ssjAug2022.Managers;

namespace pdxpartyparrot.ssjAug2022.UI
{
    public class PauseMenu : Control
    {
        private BaseButton _resumeButton;

        private BaseButton _windowedButton;

        private BaseButton _fullscreenButton;

        #region Godot Lifecycle

        public override void _Ready()
        {
            var canvas = GetNode<CanvasLayer>("CanvasLayer");

            _resumeButton = canvas.GetNode<BaseButton>("Pivot/VBoxContainer/Resume");
            _resumeButton.GrabFocus();

            _windowedButton = canvas.GetNode<BaseButton>("Pivot/VBoxContainer/Windowed");
            _fullscreenButton = canvas.GetNode<BaseButton>("Pivot/VBoxContainer/Fullscreen");
            UpdateFullscreenButtons();
        }

        public override void _EnterTree()
        {
            if(_resumeButton != null) {
                _resumeButton.GrabFocus();
            }
        }

        #endregion

        private void UpdateFullscreenButtons()
        {
            _windowedButton.Visible = PartyParrotManager.Instance.IsFullscreen;
            _fullscreenButton.Visible = !PartyParrotManager.Instance.IsFullscreen;
        }

        #region Signal Handlers

        private void _on_Resume_pressed()
        {
            PartyParrotManager.Instance.TogglePause();
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

        private void _on_Quit_pressed()
        {
            PartyParrotManager.Instance.SafeQuit();
        }

        #endregion
    }
}

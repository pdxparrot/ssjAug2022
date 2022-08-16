using Godot;

using pdxpartyparrot.ssjAug2022.Managers;

namespace pdxpartyparrot.ssjAug2022.UI
{
    public class PlayerHUD : Control
    {
        private CanvasLayer _canvas;

        // TODO: temp hack
        public Label _timer;

        #region Godot Lifecycle

        public override void _Ready()
        {
            _canvas = GetNode<CanvasLayer>("CanvasLayer");

            // TODO: temp hack
            _timer = GetNode<Label>("CanvasLayer/VBoxContainer/TEMP TIMER");
        }

        public override void _Process(float delta)
        {
            // TODO: temp hack
            if(GameManager.Instance.IsGameOver) {
                _timer.Text = "0";
            } else {
                _timer.Text = $"{GameManager.Instance.TimeRemaining}";
            }
        }

        #endregion

        public void HideHUD()
        {
            _canvas.Hide();
        }

        public void ShowHUD()
        {
            _canvas.Show();
        }
    }
}

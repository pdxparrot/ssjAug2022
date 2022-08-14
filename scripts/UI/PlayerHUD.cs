using Godot;

using pdxpartyparrot.ssjAug2022.Managers;

namespace pdxpartyparrot.ssjAug2022.UI
{
    public class PlayerHUD : Control
    {
        // TODO: temp hack
        [Export]
        public Label _timer;

        #region Godot Lifecycle

        public override void _Ready()
        {
            _timer = GetNode<Label>("TEMP TIMER");
        }

        public override void _Process(float delta)
        {
            if(GameManager.Instance.IsGameOver) {
                _timer.Text = "0";
            } else {
                _timer.Text = $"{GameManager.Instance.TimeRemaining}";
            }
        }

        #endregion
    }
}

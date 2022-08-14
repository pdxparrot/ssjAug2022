using Godot;

namespace pdxpartyparrot.ssjAug2022
{
    public class PlayerHUD : Control
    {
        // TODO: temp hack
        [Export]
        public Label _timer;

        private GameManager _gameManager;

        #region Godot Lifecycle

        public override void _Ready()
        {
            _gameManager = GetNode<GameManager>("/root/GameManager");

            _timer = GetNode<Label>("TEMP TIMER");
        }

        public override void _Process(float delta)
        {
            if(_gameManager.IsGameOver) {
                _timer.Text = "0";
            } else {
                _timer.Text = $"{_gameManager.TimeRemaining}";
            }
        }

        #endregion
    }
}

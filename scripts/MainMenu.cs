using Godot;

using pdxpartyparrot.ssjAug2022.Managers;

namespace pdxpartyparrot.ssjAug2022
{
    public class MainMenu : Node
    {
        private GameManager _gameManager;

        #region Godot Lifecycle

        public override void _Ready()
        {
            _gameManager = GetNode<GameManager>("/root/GameManager");
        }

        #endregion

        #region Signals

        private void _on_Play_pressed()
        {
            _gameManager.StartGame();
        }

        #endregion
    }
}

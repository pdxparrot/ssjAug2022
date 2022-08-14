using Godot;

namespace pdxpartyparrot.ssjAug2022
{
    // singleton
    public class GameManager : Node
    {
        [Export]
        private PackedScene _mainMenuScene;

        [Export]
        private PackedScene _initialLevelScene;

        private bool _isGameOver;

        public bool IsGameOver => _isGameOver;

        private SceneManager _sceneManager;

        #region Godot Lifecycle

        public override void _Ready()
        {
            _sceneManager = GetNode<SceneManager>("/root/SceneManager");

            _isGameOver = false;
        }

        #endregion

        public void StartGame()
        {
            GD.Print("[GameManager] Starting game ...");

            _sceneManager.LoadLevel(_initialLevelScene);
        }

        public void GameOver()
        {
            GD.Print("[GameManager] Game over!");

            _sceneManager.LoadLevel(_mainMenuScene);
        }
    }
}

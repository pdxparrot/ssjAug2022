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

        // TODO: temp hack
        private float _timer;
        public int TimeRemaining => (int)_timer;

        private SceneManager _sceneManager;

        #region Godot Lifecycle

        public override void _Ready()
        {
            _sceneManager = GetNode<SceneManager>("/root/SceneManager");

            _isGameOver = false;
        }

        public override void _Process(float delta)
        {
            // TODO: temp hack
            if(_timer > 0.0f) {
                _timer -= delta;
                if(_timer <= 0.0f) {
                    GameOver();
                }
            }
        }

        #endregion

        public void StartGame()
        {
            GD.Print("[GameManager] Starting game ...");

            _sceneManager.LoadLevel(_initialLevelScene, () => _timer = 30.0f);
        }

        public void GameOver()
        {
            GD.Print("[GameManager] Game over!");

            _sceneManager.LoadLevel(_mainMenuScene);
        }
    }
}

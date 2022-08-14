using Godot;

using pdxpartyparrot.ssjAug2022.Util;

namespace pdxpartyparrot.ssjAug2022.Managers
{
    public class GameManager : SingletonNode<GameManager>
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

        #region Godot Lifecycle

        public override void _Ready()
        {
            base._Ready();

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

            SceneManager.Instance.LoadLevel(_initialLevelScene, () => _timer = 30.0f);
        }

        public void GameOver()
        {
            GD.Print("[GameManager] Game over!");

            SceneManager.Instance.LoadLevel(_mainMenuScene);
        }
    }
}

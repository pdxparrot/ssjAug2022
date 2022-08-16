using Godot;

using System.Threading.Tasks;

using pdxpartyparrot.ssjAug2022.Util;

namespace pdxpartyparrot.ssjAug2022.Managers
{
    public class GameManager : SingletonNode<GameManager>
    {
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

        public override async void _Process(float delta)
        {
            // TODO: temp hack
            if(_timer > 0.0f) {
                _timer -= delta;
                if(_timer <= 0.0f) {
                    await GameOverAsync().ConfigureAwait(false);
                }
            }
        }

        #endregion

        public async Task StartGameAsync()
        {
            GD.Print("[GameManager] Starting game ...");

            await SceneManager.Instance.LoadInitialLevelAsync(() => _timer = 30.0f).ConfigureAwait(false);
        }

        public async Task GameOverAsync()
        {
            GD.Print("[GameManager] Game over!");

            PlayerManager.Instance.DestroyPlayers();

            await SceneManager.Instance.LoadMainMenuAsync().ConfigureAwait(false);
        }
    }
}

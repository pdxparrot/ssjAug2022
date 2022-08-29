using Godot;

using System.Threading.Tasks;

using pdxpartyparrot.ssjAug2022.Util;

namespace pdxpartyparrot.ssjAug2022.Managers
{
    public class GameManager : SingletonNode<GameManager>
    {
        private bool _isGameOver;

        public bool IsGameOver => _isGameOver;

        private Timer _gameOverTimer;

        #region Godot Lifecycle

        public override void _Ready()
        {
            base._Ready();

            _gameOverTimer = GetNode<Timer>("Timers/Game Over");

            _isGameOver = false;
        }

        #endregion

        public async Task StartGameAsync()
        {
            GD.Print("[GameManager] Starting game ...");

            ViewerManager.Instance.InstanceViewers(1);

            await SceneManager.Instance.LoadInitialLevelAsync().ConfigureAwait(false);
        }

        public void GameOver()
        {
            GD.Print("[GameManager] Game over!");

            _isGameOver = true;

            _gameOverTimer.Start();
        }

        public void EnemyDefeated()
        {
            if(NPCManager.Instance.NPCCount == 0) {
                GD.Print("[GameManager] All enemies defeated!");

                GameOver();
            }
        }

        #region Signal Handlers

        private async void _on_Game_Over_timeout()
        {
            _isGameOver = false;

            PlayerManager.Instance.DestroyPlayers();
            NPCManager.Instance.DespawnAllNPCs(true);

            await SceneManager.Instance.LoadMainMenuAsync().ConfigureAwait(false);
        }

        #endregion
    }
}

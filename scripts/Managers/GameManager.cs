using Godot;

using System.Threading.Tasks;

using pdxpartyparrot.ssjAug2022.Util;

namespace pdxpartyparrot.ssjAug2022.Managers
{
    public class GameManager : SingletonNode<GameManager>
    {
        private bool _isGameOver;

        public bool IsGameOver => _isGameOver;

        #region Godot Lifecycle

        public override void _Ready()
        {
            base._Ready();

            _isGameOver = false;
        }

        #endregion

        public async Task StartGameAsync()
        {
            GD.Print("[GameManager] Starting game ...");

            ViewerManager.Instance.InstanceViewers(1);

            await SceneManager.Instance.LoadInitialLevelAsync().ConfigureAwait(false);
        }

        public async Task GameOverAsync()
        {
            GD.Print("[GameManager] Game over!");

            PlayerManager.Instance.DestroyPlayers();
            NPCManager.Instance.DespawnAllNPCs(true);

            await SceneManager.Instance.LoadMainMenuAsync().ConfigureAwait(false);
        }

        public async Task EnemyDefeatedAsync()
        {
            if(NPCManager.Instance.NPCCount == 0) {
                GD.Print("[GameManager] All enemies defeated!");

                await GameOverAsync();
            }
        }
    }
}

using Godot;

using pdxpartyparrot.ssjAug2022.Camera;
using pdxpartyparrot.ssjAug2022.Managers;

namespace pdxpartyparrot.ssjAug2022
{
    public class LevelHelper : Node
    {
        [Export]
        private int _enemiesToSpawn;

        [Export]
        private PackedScene _humanScene;

        private FollowCamera _viewer;

        #region Godot Lifecycle

        public override void _Ready()
        {
            SpawnManager.Instance.Initialize();

            GameUIManager.Instance.ShowHUD();

            _viewer = ViewerManager.Instance.AcquireViewer<FollowCamera>();

            var player = PlayerManager.Instance.SpawnPlayer(0);
            _viewer.Follow(player);

            SpawnEnemies();
        }

        public override void _ExitTree()
        {
            ViewerManager.Instance.ReleaseViewer(_viewer);
            _viewer = null;

            GameUIManager.Instance.HideHUD();
        }

        #endregion

        private void SpawnEnemy()
        {
            // TODO:
        }

        private void SpawnEnemies()
        {
            GD.Print($"[Level] Spawning {_enemiesToSpawn} enemies ...");

            for(int i = 0; i < _enemiesToSpawn; ++i) {
                SpawnEnemy();
            }
        }
    }
}

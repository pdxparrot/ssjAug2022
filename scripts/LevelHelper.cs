using Godot;

using System;

using pdxpartyparrot.ssjAug2022.Camera;
using pdxpartyparrot.ssjAug2022.Managers;
using pdxpartyparrot.ssjAug2022.NPCs;

namespace pdxpartyparrot.ssjAug2022
{
    public class LevelHelper : Node
    {
        [Export]
        private int _enemiesToSpawn;

        [Export]
        private string _enemySpawnTag = string.Empty;

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

        private SimpleNPC SpawnEnemy()
        {
            var id = Guid.NewGuid();

            GD.Print($"[Level] Spawning enemy {id}...");

            var spawnPoint = SpawnManager.Instance.GetSpawnPoint(_enemySpawnTag);
            if(null == spawnPoint) {
                GD.PushError("Failed to get enemy spawnpoint!");
                return null;
            }

            var enemy = spawnPoint.SpawnNPC(_humanScene, $"Human {id}");
            enemy.Id = id;

            return enemy;
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

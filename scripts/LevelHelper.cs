using Godot;

using System;

using pdxpartyparrot.ssjAug2022.Camera;
using pdxpartyparrot.ssjAug2022.Managers;

namespace pdxpartyparrot.ssjAug2022
{
    public class LevelHelper : Node
    {
        private enum Stage
        {
            Enemies,
            Boss,
        }

        #region Enemies

        [Export]
        private int _enemiesToSpawn;

        [Export]
        private string _enemySpawnTag = string.Empty;

        [Export]
        private PackedScene _humanScene;

        #endregion

        #region Boss

        [Export]
        private string _bossSpawnTag = string.Empty;

        [Export]
        private PackedScene _bossScene;

        #endregion

        private AudioStreamPlayer _musicPlayer;

        private FollowCamera _viewer;

        private Stage _stage = Stage.Enemies;

        #region Godot Lifecycle

        public override void _Ready()
        {
            _musicPlayer = GetNode<AudioStreamPlayer>("Music");
            _musicPlayer.Play();

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
            var id = Guid.NewGuid();

            //GD.Print($"[Level] Spawning enemy {id}...");

            var spawnPoint = SpawnManager.Instance.GetSpawnPoint(_enemySpawnTag);
            if(null == spawnPoint) {
                GD.PushError("Failed to get enemy spawnpoint!");
                return;
            }

            var enemy = spawnPoint.SpawnNPC(_humanScene, $"Human {id}");
            enemy.Id = id;
        }

        private void SpawnEnemies()
        {
            GD.Print($"[Level] Spawning {_enemiesToSpawn} enemies ...");

            for(int i = 0; i < _enemiesToSpawn; ++i) {
                SpawnEnemy();
            }
        }

        private void SpawnBoss()
        {
            GD.Print($"[Level] Spawning boss ...");

            var id = Guid.NewGuid();

            //GD.Print($"[Level] Spawning boss {id}...");

            var spawnPoint = SpawnManager.Instance.GetSpawnPoint(_bossSpawnTag);
            if(null == spawnPoint) {
                GD.PushError("Failed to get boss spawnpoint!");
                return;
            }

            var boss = spawnPoint.SpawnNPC(_bossScene, $"Boss {id}");
            boss.Id = id;
        }
    }
}

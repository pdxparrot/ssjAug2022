using Godot;

using System;

using pdxpartyparrot.ssjAug2022.Camera;
using pdxpartyparrot.ssjAug2022.Managers;
using pdxpartyparrot.ssjAug2022.NPCs.Boss;
using pdxpartyparrot.ssjAug2022.NPCs.Human;

namespace pdxpartyparrot.ssjAug2022
{
    public class LevelHelper : Node
    {
        public enum Stage
        {
            Enemies,
            Boss,
        }

        #region Events

        public event EventHandler<EventArgs> StageChangeEvent;

        #endregion

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
            GameManager.Instance.Level = this;

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

        public override void _UnhandledInput(InputEvent @event)
        {
            if(!PartyParrotManager.Instance.IsEditor) {
                return;
            }

            if(GameManager.Instance.IsGameOver) {
                return;
            }

            if(@event is InputEventKey eventKey) {
                if(eventKey.Pressed && eventKey.Scancode == (int)KeyList.K) {
                    switch(_stage) {
                    case Stage.Enemies:
                        KillAllEnemies();
                        break;
                    case Stage.Boss:
                        KillBoss();
                        break;
                    }
                }
            }
        }

        #endregion

        public void EnemyDefeated()
        {
            switch(_stage) {
            case Stage.Enemies:
                if(NPCManager.Instance.NPCCount == 0) {
                    GD.Print("[GameManager] All enemies defeated!");

                    EnterBossStage();
                }
                break;
            case Stage.Boss:
                if(NPCManager.Instance.NPCCount == 0) {
                    GD.Print("[Level] Boss defeated!");

                    GameManager.Instance.GameOver(true);
                }
                break;
            }
        }

        private void EnterBossStage()
        {
            _stage = Stage.Boss;

            StageChangeEvent?.Invoke(this, EventArgs.Empty);

            SpawnBoss();
        }

        #region Enemies

        private void KillAllEnemies()
        {
            GD.Print("[Level] Killing all enemies ...");

            foreach(var npc in NPCManager.Instance.NPCs) {
                if(npc is Human human) {
                    human.Kill();
                }
            }
        }

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

        #endregion

        #region Boss

        private void KillBoss()
        {
            GD.Print("[Level] Killing boss ...");

            foreach(var npc in NPCManager.Instance.NPCs) {
                if(npc is Boss boss) {
                    boss.Kill();
                }
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

        #endregion
    }
}

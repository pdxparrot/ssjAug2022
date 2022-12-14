using Godot;

using System;
using System.Threading.Tasks;

using pdxpartyparrot.ssjAug2022.Util;

namespace pdxpartyparrot.ssjAug2022.Managers
{
    public class GameManager : SingletonNode<GameManager>
    {
        #region Events

        public event EventHandler<EventArgs> GameOverEvent;

        #endregion

        [Export]
        private PackedScene _gameOverLossScene;

        private Control _gameOverLossUI;

        [Export]
        private PackedScene _gameOverWinScene;

        private Control _gameOverWinUI;

        private bool _isGameOver;

        public bool IsGameOver => _isGameOver;

        private Timer _gameOverTimer;

        public LevelHelper Level { get; set; }

        #region Godot Lifecycle

        public override void _Ready()
        {
            base._Ready();

            _gameOverLossUI = (Control)_gameOverLossScene.Instance();
            _gameOverWinUI = (Control)_gameOverWinScene.Instance();

            _gameOverTimer = GetNode<Timer>("Timers/Game Over");

            _isGameOver = false;
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if(IsGameOver) {
                return;
            }

            if(@event.IsActionPressed("pause") && !PartyParrotManager.Instance.IsPaused) {
                PartyParrotManager.Instance.TogglePause();
            }
        }

        #endregion

        public async Task StartGameAsync()
        {
            GD.Print("[GameManager] Starting game ...");

            ViewerManager.Instance.InstanceViewers(1);

            await SceneManager.Instance.LoadInitialLevelAsync().ConfigureAwait(false);
        }

        private void ShowGameOverUI(bool win)
        {
            if(win) {
                AddChild(_gameOverWinUI);
            } else {
                AddChild(_gameOverLossUI);
            }
        }

        private void HideGameOverUI()
        {
            RemoveChild(_gameOverLossUI);
            RemoveChild(_gameOverWinUI);
        }

        public void GameOver(bool win)
        {
            GD.Print("[GameManager] Game over!");

            _isGameOver = true;

            ShowGameOverUI(win);

            _gameOverTimer.Start();

            GameOverEvent?.Invoke(this, EventArgs.Empty);
        }

        #region Signal Handlers

        private async void _on_Game_Over_timeout()
        {
            _isGameOver = false;

            HideGameOverUI();

            PlayerManager.Instance.DestroyPlayers();
            NPCManager.Instance.DespawnAllNPCs(true);

            await SceneManager.Instance.LoadMainMenuAsync().ConfigureAwait(false);
        }

        #endregion
    }
}

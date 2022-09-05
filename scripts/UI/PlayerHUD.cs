using Godot;

using System;

using pdxpartyparrot.ssjAug2022.Managers;

namespace pdxpartyparrot.ssjAug2022.UI
{
    public class PlayerHUD : Control
    {
        [Export]
        private float _animationSeconds = 0.25f;

        private CanvasLayer _canvas;

        private Control _enemiesStageContainer;

        private Control _bossStageContainer;

        #region Player Health

        private TextureProgress _playerHealthBar;

        private TextureProgress _playerBossHealthBar;

        private float _playerHealthPercent = 100.0f;

        private Tween _playerHealthTween;

        #endregion

        #region Boss Health

        private TextureProgress _bossHealthBar;

        private float _bossHealthPercent = 100.0f;

        private Tween _bossHealthTween;

        #endregion

        #region Godot Lifecycle

        public override void _Ready()
        {
            _canvas = GetNode<CanvasLayer>("CanvasLayer");

            _enemiesStageContainer = _canvas.GetNode<Control>("Pivot/Stage Enemies");

            _playerHealthBar = _enemiesStageContainer.GetNode<TextureProgress>("Player Health");
            _playerHealthBar.MinValue = 0.0;

            _bossStageContainer = _canvas.GetNode<Control>("Pivot/Stage Boss");

            _playerBossHealthBar = _bossStageContainer.GetNode<TextureProgress>("Player Health");
            _playerBossHealthBar.MinValue = 0.0;

            _bossHealthBar = _bossStageContainer.GetNode<TextureProgress>("Boss Health");
            _bossHealthBar.MinValue = 0.0;

            _playerHealthTween = GetNode<Tween>("Tweens/Player Health");
            _bossHealthTween = GetNode<Tween>("Tweens/Boss Health");
        }

        public override void _EnterTree()
        {
            GameManager.Instance.Level.StageChangeEvent += StageChangeEventHandler;
        }

        public override void _ExitTree()
        {
            GameManager.Instance.Level.StageChangeEvent -= StageChangeEventHandler;
        }

        public override void _Process(float delta)
        {
            // TODO: we should only update these if they actually changed
            _playerHealthBar.Value = _playerHealthPercent;
            _playerBossHealthBar.Value = _playerHealthPercent;
            _bossHealthBar.Value = _bossHealthPercent;
        }

        #endregion

        public void HideHUD()
        {
            _canvas.Hide();
        }

        public void ShowHUD()
        {
            _canvas.Show();
        }

        private void UpdateHUDStage()
        {
            switch(GameManager.Instance.Level.CurrentStage) {
            case LevelHelper.Stage.Enemies:
                _playerHealthPercent = 100.0f;

                _enemiesStageContainer.Show();
                _bossStageContainer.Hide();
                break;
            case LevelHelper.Stage.Boss:
                _bossHealthPercent = 100.0f;

                _enemiesStageContainer.Hide();
                _bossStageContainer.Show();
                break;
            }
        }

        public void UpdatePlayerHealth(float percent)
        {
            float value = percent * 100.0f;
            _playerHealthTween.InterpolateProperty(this, "_playerHealthPercent", _playerHealthPercent, value, _animationSeconds, easeType: Tween.EaseType.In);

            if(!_playerHealthTween.IsActive()) {
                _playerHealthTween.Start();
            }
        }

        public void UpdateBossHealth(float percent)
        {
            float value = percent * 100.0f;
            _bossHealthTween.InterpolateProperty(this, "_bossHealthPercent", _bossHealthPercent, value, _animationSeconds, easeType: Tween.EaseType.In);

            if(!_bossHealthTween.IsActive()) {
                _bossHealthTween.Start();
            }
        }

        #region Event Handlers

        private void StageChangeEventHandler(object sender, EventArgs args)
        {
            UpdateHUDStage();
        }

        #endregion
    }
}

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

        AnimationPlayer _bossStageAnimation;

        #region Godot Lifecycle

        public override void _Ready()
        {
            _canvas = GetNode<CanvasLayer>("CanvasLayer");

            _playerHealthBar = _canvas.GetNode<TextureProgress>("Pivot/Stage Enemies/Player Health");
            _playerHealthBar.MinValue = 0.0;

            _playerBossHealthBar = _canvas.GetNode<TextureProgress>("Pivot/Stage Boss/Player Health Panel/Player Health");
            _playerBossHealthBar.MinValue = 0.0;

            _bossHealthBar = _canvas.GetNode<TextureProgress>("Pivot/Stage Boss/Boss Health Panel/Boss Health");
            _bossHealthBar.MinValue = 0.0;

            _playerHealthTween = GetNode<Tween>("Tweens/Player Health");
            _bossHealthTween = GetNode<Tween>("Tweens/Boss Health");

            _bossStageAnimation = GetNode<AnimationPlayer>("Boss Fight UI Animation");
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

                // TODO: idk what to do here, this errors every time
                // but it's the only way to get the thing to actually reset
                _bossStageAnimation.Stop();
                //if(!string.IsNullOrWhiteSpace(_bossStageAnimation.CurrentAnimation)) {
                _bossStageAnimation.Seek(0, true);
                //}
                break;
            case LevelHelper.Stage.Boss:
                _bossHealthPercent = 100.0f;

                _bossStageAnimation.Play("lifebar_center_bottom");
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

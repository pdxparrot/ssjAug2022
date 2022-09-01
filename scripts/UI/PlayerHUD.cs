using Godot;

using pdxpartyparrot.ssjAug2022.Managers;

namespace pdxpartyparrot.ssjAug2022.UI
{
    public class PlayerHUD : Control
    {
        [Export]
        private float _animationSeconds = 0.25f;

        private CanvasLayer _canvas;

        private TextureProgress _playerHealthBar;

        private float _playerHealthPercent = 100.0f;

        private Tween _playerHealthTween;

        #region Godot Lifecycle

        public override void _Ready()
        {
            _canvas = GetNode<CanvasLayer>("CanvasLayer");

            _playerHealthBar = _canvas.GetNode<TextureProgress>("Pivot/Player Health");
            _playerHealthBar.MinValue = 0.0;

            _playerHealthTween = GetNode<Tween>("Tweens/Player Health");
        }

        public override void _Process(float delta)
        {
            _playerHealthBar.Value = _playerHealthPercent;
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

        public void UpdatePlayerHealth(float percent)
        {
            float value = percent * 100.0f;
            _playerHealthTween.InterpolateProperty(this, "_playerHealthPercent", _playerHealthPercent, value, _animationSeconds, easeType: Tween.EaseType.In);

            if(!_playerHealthTween.IsActive()) {
                _playerHealthTween.Start();
            }
        }
    }
}

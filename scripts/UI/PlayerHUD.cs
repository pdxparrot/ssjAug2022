using Godot;

using pdxpartyparrot.ssjAug2022.Managers;

namespace pdxpartyparrot.ssjAug2022.UI
{
    public class PlayerHUD : Control
    {
        private CanvasLayer _canvas;

        private TextureProgress _health;

        #region Godot Lifecycle

        public override void _Ready()
        {
            _canvas = GetNode<CanvasLayer>("CanvasLayer");

            _health = _canvas.GetNode<TextureProgress>("Pivot/Health");
            _health.MinValue = 0;
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

        public void SetMaxHealth(int maxHealth)
        {
            _health.MaxValue = maxHealth;
        }

        public void UpdateHealth(int currentHealth)
        {
            _health.Value = currentHealth;
        }
    }
}

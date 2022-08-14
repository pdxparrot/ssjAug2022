using Godot;

using pdxpartyparrot.ssjAug2022.UI;

namespace pdxpartyparrot.ssjAug2022.Managers
{
    // singleton
    public class GameUIManager : Node
    {
        [Export]
        private PackedScene _playerHUDScene;

        private PlayerHUD _playerHUD;

        #region Godot Lifecycle

        public override void _Ready()
        {
            CreatePlayerHUD();
        }

        #endregion

        private void CreatePlayerHUD()
        {
            var playerHUD = _playerHUDScene.Instance();
            AddChild(playerHUD);

            _playerHUD = (PlayerHUD)playerHUD;
            _playerHUD.Hide();
        }

        public void ShowHUD()
        {
            _playerHUD.Show();
        }

        public void HideHUD()
        {
            _playerHUD.Hide();
        }
    }
}
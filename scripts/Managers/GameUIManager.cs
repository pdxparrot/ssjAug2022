using Godot;

using pdxpartyparrot.ssjAug2022.UI;
using pdxpartyparrot.ssjAug2022.Util;

namespace pdxpartyparrot.ssjAug2022.Managers
{
    public class GameUIManager : SingletonNode<GameUIManager>
    {
        [Export]
        private PackedScene _playerHUDScene;

        private PlayerHUD _playerHUD;

        #region Godot Lifecycle

        public override void _Ready()
        {
            base._Ready();

            CreatePlayerHUD();
        }

        #endregion

        private void CreatePlayerHUD()
        {
            var playerHUD = _playerHUDScene.Instance();
            playerHUD.Name = "Player HUD";
            AddChild(playerHUD);

            _playerHUD = (PlayerHUD)playerHUD;
            _playerHUD.HideHUD();
        }

        public void ShowHUD()
        {
            _playerHUD.ShowHUD();
        }

        public void HideHUD()
        {
            _playerHUD.HideHUD();
        }
    }
}

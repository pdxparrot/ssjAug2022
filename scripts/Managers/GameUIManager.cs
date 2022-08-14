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

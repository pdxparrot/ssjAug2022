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
            if(IsInstanceValid(_playerHUD)) {
                GD.PushWarning("[GameUIManager] Re-creating HUD ...");

                _playerHUD.QueueFree();
            }

            _playerHUD = (PlayerHUD)_playerHUDScene.Instance();
            _playerHUD.Name = "Player HUD";
        }

        public void ShowHUD()
        {
            AddChild(_playerHUD);
        }

        public void HideHUD()
        {
            RemoveChild(_playerHUD);
        }
    }
}

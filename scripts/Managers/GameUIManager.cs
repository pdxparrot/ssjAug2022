using Godot;

using pdxpartyparrot.ssjAug2022.UI;
using pdxpartyparrot.ssjAug2022.Util;

namespace pdxpartyparrot.ssjAug2022.Managers
{
    public class GameUIManager : SingletonNode<GameUIManager>
    {
        [Export]
        private PackedScene _playerHUDScene;

        public PlayerHUD HUD { get; private set; }

        #region Godot Lifecycle

        public override void _Ready()
        {
            base._Ready();

            CreatePlayerHUD();
        }

        #endregion

        private void CreatePlayerHUD()
        {
            if(IsInstanceValid(HUD)) {
                GD.PushWarning("[GameUIManager] Re-creating HUD ...");

                HUD.QueueFree();
            }

            HUD = (PlayerHUD)_playerHUDScene.Instance();
            HUD.Name = "Player HUD";
        }

        public void ShowHUD()
        {
            AddChild(HUD);
        }

        public void HideHUD()
        {
            RemoveChild(HUD);
        }
    }
}

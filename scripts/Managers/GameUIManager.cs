using Godot;

using System;

using pdxpartyparrot.ssjAug2022.UI;
using pdxpartyparrot.ssjAug2022.Util;

namespace pdxpartyparrot.ssjAug2022.Managers
{
    public class GameUIManager : SingletonNode<GameUIManager>
    {
        [Export]
        private PackedScene _pauseMenuScene;

        [Export]
        private PackedScene _playerHUDScene;

        private PauseMenu _pauseMenu;

        public PlayerHUD HUD { get; private set; }

        #region Godot Lifecycle

        public override void _Ready()
        {
            base._Ready();

            CreatePauseMenu();
            CreatePlayerHUD();
        }

        public override void _EnterTree()
        {
            base._EnterTree();

            PartyParrotManager.Instance.PauseEvent += PauseEventHandler;
        }

        public override void _ExitTree()
        {
            base._ExitTree();

            if(PartyParrotManager.HasInstance) {
                PartyParrotManager.Instance.PauseEvent -= PauseEventHandler;
            }
        }

        #endregion

        private void CreatePauseMenu()
        {
            if(IsInstanceValid(_pauseMenu)) {
                GD.PushWarning("[GameUIManager] Re-creating pause menu ...");

                _pauseMenu.QueueFree();
            }

            _pauseMenu = (PauseMenu)_pauseMenuScene.Instance();
            _pauseMenu.Name = "Pause Menu";
        }

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

        private void TogglePauseMenu()
        {
            if(PartyParrotManager.Instance.IsPaused) {
                AddChild(_pauseMenu);
            } else {
                RemoveChild(_pauseMenu);
            }
        }

        #region Event Handlers

        private void PauseEventHandler(object sender, EventArgs args)
        {
            TogglePauseMenu();
        }

        #endregion
    }
}

using Godot;

namespace pdxpartyparrot.ssjAug2022
{
    public class Level : Node
    {
        private GameUIManager _gameUIManager;

        #region Godot Lifecycle

        public override void _Ready()
        {
            _gameUIManager = GetNode<GameUIManager>("/root/GameUiManager");
            _gameUIManager.ShowHUD();
        }

        #endregion
    }
}

using Godot;

namespace pdxpartyparrot.ssjAug2022
{
    public class Level : Node
    {
        [Export]
        private PackedScene _playerHUDScene;

        #region Godot Lifecycle

        public override void _Ready()
        {
            var playerHUD = _playerHUDScene.Instance();
            AddChild(playerHUD);
        }

        #endregion
    }
}

using Godot;

namespace pdxpartyparrot.ssjAug2022.Managers
{
    // singleton
    public class SpawnManager : Node
    {
        [Export]
        private PackedScene _playerScene;

        #region Godot Lifecycle

        public override void _Ready()
        {
        }

        #endregion
    }
}

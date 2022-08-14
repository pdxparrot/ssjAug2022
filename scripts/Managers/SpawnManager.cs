using Godot;

using pdxpartyparrot.ssjAug2022.Util;

namespace pdxpartyparrot.ssjAug2022.Managers
{
    public class SpawnManager : SingletonNode<SpawnManager>
    {
        [Export]
        private PackedScene _playerScene;

        #region Godot Lifecycle

        public override void _Ready()
        {
            base._Ready();
        }

        #endregion
    }
}

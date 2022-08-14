using Godot;

using pdxpartyparrot.ssjAug2022.Managers;

namespace pdxpartyparrot.ssjAug2022.World
{
    public class SpawnPoint : Spatial
    {
        [Export]
        private string _tag = string.Empty;

        public string Tag => _tag;

        #region Godot Lifecycle

        public override void _Ready()
        {
        }

        #endregion
    }
}

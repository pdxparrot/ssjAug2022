using Godot;

using pdxpartyparrot.ssjAug2022.Camera;
using pdxpartyparrot.ssjAug2022.Managers;

namespace pdxpartyparrot.ssjAug2022
{
    public class LevelHelper : Node
    {
        private FollowCamera _viewer;

        #region Godot Lifecycle

        public override void _Ready()
        {
            SpawnManager.Instance.Initialize();

            GameUIManager.Instance.ShowHUD();

            _viewer = ViewerManager.Instance.AcquireViewer<FollowCamera>();

            var player = PlayerManager.Instance.SpawnPlayer(0);
            _viewer.Follow(player);
        }

        public override void _ExitTree()
        {
            ViewerManager.Instance.ReleaseViewer(_viewer);
            _viewer = null;

            GameUIManager.Instance.HideHUD();
        }

        #endregion
    }
}

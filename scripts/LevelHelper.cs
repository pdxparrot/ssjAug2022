using Godot;

using pdxpartyparrot.ssjAug2022.Managers;

namespace pdxpartyparrot.ssjAug2022
{
    public class LevelHelper : Node
    {
        #region Godot Lifecycle

        public override void _Ready()
        {
            SpawnManager.Instance.Initialize();

            GameUIManager.Instance.ShowHUD();

            PlayerManager.Instance.SpawnPlayer(0);
        }

        public override void _ExitTree()
        {
            GameUIManager.Instance.HideHUD();
        }

        #endregion
    }
}

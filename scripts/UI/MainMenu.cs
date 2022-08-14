using Godot;

using pdxpartyparrot.ssjAug2022.Managers;

namespace pdxpartyparrot.ssjAug2022.UI
{
    public class MainMenu : Control
    {
        #region Godot Lifecycle

        public override void _Ready()
        {

        }

        #endregion

        #region Signals

        private void _on_Play_pressed()
        {
            GameManager.Instance.StartGame();
        }

        #endregion
    }
}

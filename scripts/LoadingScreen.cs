using Godot;

namespace pdxpartyparrot.ssjAug2022
{
    public class LoadingScreen : Node
    {
        #region Godot Lifecycle

        public override void _Ready()
        {
        }

        #endregion

        public void Show()
        {
            GD.Print("[LoadingScreen] Show");
        }

        public void Hide()
        {
            GD.Print("[LoadingScreen] Hide");
        }
    }
}

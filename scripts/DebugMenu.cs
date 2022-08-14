using Godot;

using pdxpartyparrot.ssjAug2022.Util;

namespace pdxpartyparrot.ssjAug2022
{
    public class DebugMenu : SingletonUI<DebugMenu>
    {
        #region Godot Lifecycle

        public override void _Ready()
        {
            base._Ready();

            Hide();
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if(@event is InputEventKey eventKey) {
                if(eventKey.Pressed && eventKey.Scancode == (int)KeyList.Quoteleft) {
                    Toggle();
                }
            }
        }

        #endregion

        public void Toggle()
        {
            if(Visible) {
                GD.Print("[DebugMenu] Hide");
                Hide();
            } else {
                GD.Print("[DebugMenu] Show");
                Show();
            }
        }
    }
}

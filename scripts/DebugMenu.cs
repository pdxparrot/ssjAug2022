using Godot;

namespace pdxpartyparrot.ssjAug2022
{
    // singleton UI
    public class DebugMenu : Control
    {
        #region Godot Lifecycle

        public override void _Ready()
        {
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

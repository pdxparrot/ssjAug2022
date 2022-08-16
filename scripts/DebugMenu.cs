using Godot;

using pdxpartyparrot.ssjAug2022.Util;

namespace pdxpartyparrot.ssjAug2022
{
    public class DebugMenu : SingletonUI<DebugMenu>
    {
        private CanvasLayer _canvas;

        private Label _fps;

        #region Godot Lifecycle

        public override void _Ready()
        {
            base._Ready();

            _canvas = GetNode<CanvasLayer>("CanvasLayer");
            _fps = GetNode<Label>("CanvasLayer/HBoxContainer/FPS");

            _canvas.Hide();
        }

        public override void _Process(float delta)
        {
            if(!_canvas.Visible) {
                return;
            }

            _fps.Text = $"{Engine.GetFramesPerSecond()}";
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
            if(_canvas.Visible) {
                GD.Print("[DebugMenu] Hide");
                _canvas.Hide();
            } else {
                GD.Print("[DebugMenu] Show");
                _canvas.Show();
            }
        }
    }
}

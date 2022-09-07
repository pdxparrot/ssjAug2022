using Godot;

using pdxpartyparrot.ssjAug2022.Managers;
using pdxpartyparrot.ssjAug2022.Util;

namespace pdxpartyparrot.ssjAug2022
{
    public class DebugMenu : SingletonUI<DebugMenu>
    {
        private CanvasLayer _canvas;

        private Label _fps;

        private Label _staticMemory;

        private Label _staticMemoryMax;

        private Label _dynamicMemory;

        private Label _dynamicMemoryMax;

        #region Godot Lifecycle

        public override void _Ready()
        {
            base._Ready();

            _canvas = GetNode<CanvasLayer>("CanvasLayer");

            // TODO: all of these should be created dynamically
            // so the debug menu can be code-driven
            _fps = _canvas.GetNode<Label>("VBoxContainer/FPS/FPS");
            _staticMemory = _canvas.GetNode<Label>("VBoxContainer/Static Memory/Static Memory");
            _staticMemoryMax = _canvas.GetNode<Label>("VBoxContainer/Static Memory/Static Memory Max");
            _dynamicMemory = _canvas.GetNode<Label>("VBoxContainer/Dynamic Memory/Dynamic Memory");
            _dynamicMemoryMax = _canvas.GetNode<Label>("VBoxContainer/Dynamic Memory/Dynamic Memory Max");

            _canvas.Hide();
        }

        public override void _Process(float delta)
        {
            if(!_canvas.Visible) {
                return;
            }

            _fps.Text = $"{Performance.GetMonitor(Performance.Monitor.TimeFps)}";
            //frame time = Performance.GetMonitor(Performance.Monitor.TimeProcess)
            //physics time = Performance.GetMonitor(Performance.Monitor.TimePhysicsProcess)
            //draw calls (3d) = Performance.GetMonitor(Performance.Monitor.RenderDrawCallsInFrame)
            //drawcalls (2d) = Performance.GetMonitor(Performance.Monitor.Render2dDrawCallsInFrame)
            _staticMemory.Text = $"{Performance.GetMonitor(Performance.Monitor.MemoryStatic) / 1048576.0f:0.00}MB";
            _staticMemoryMax.Text = $"({Performance.GetMonitor(Performance.Monitor.MemoryStaticMax) / 1048576.0f:0.00}MB)";
            _dynamicMemory.Text = $"({Performance.GetMonitor(Performance.Monitor.MemoryDynamic) / 1048576.0f:0.00}MB)";
            _dynamicMemoryMax.Text = $"({Performance.GetMonitor(Performance.Monitor.MemoryDynamicMax) / 1048576.0f:0.00}MB)";
            //video memory used = Performance.GetMonitor(Performance.Monitor.RenderVideoMemUsed)
            //node count = Performance.GetMonitor(Performance.Monitor.ObjectNodeCount)
            //orphaned node count = Performance.GetMonitor(Performance.Monitor.ObjectOrphanNodeCount)
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

        #region Signal Handlers

        private void _on_Debug_Overlay_pressed()
        {
            DebugOverlay.Instance.Toggle();
        }

        private void _on_Quit_pressed()
        {
            PartyParrotManager.Instance.SafeQuit();
        }

        #endregion
    }
}

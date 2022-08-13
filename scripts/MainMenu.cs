using Godot;


namespace pdxpartyparrot.ssjAug2022
{
    public class MainMenu : Node
    {
        [Export]
        private string _initialLevel = "level01.tscn";

        #region Godot Lifecycle

        public override void _Ready()
        {
        }

        #endregion

        private void LoadInitialLevel()
        {
            GD.Print($"Loading initial level ${_initialLevel}...");
            GetTree().ChangeScene($"res://scenes/{_initialLevel}");
        }

        #region Signals

        private void _on_Play_pressed()
        {
            LoadInitialLevel();
        }

        #endregion
    }
}

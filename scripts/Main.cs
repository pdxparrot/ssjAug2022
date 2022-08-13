using Godot;


namespace pdxpartyparrot.ssjAug2022
{
    public class Main : Node
    {
        [Export]
        private string _mainLevel = "level01.tscn";

        public override void _Ready()
        {
            GetTree().ChangeScene($"res://scenes/{_mainLevel}");
        }
    }
}

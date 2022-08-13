using Godot;

namespace pdxpartyparrot.ssjAug2022
{
    public class SceneManager : Node
    {
        #region Godot Lifecycle

        public override void _Ready()
        {
        }

        #endregion

        public void LoadLevel(PackedScene level)
        {
            GD.Print($"Loading level {level.ResourcePath}...");
            GetTree().ChangeSceneTo(level);
        }
    }
}

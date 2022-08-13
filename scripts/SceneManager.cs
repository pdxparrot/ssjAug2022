using Godot;

namespace pdxpartyparrot.ssjAug2022
{
    public class SceneManager : Node
    {
        private LoadingScreen _loadingScreen;

        #region Godot Lifecycle

        public override void _Ready()
        {
            _loadingScreen = GetNode<LoadingScreen>("/root/LoadingScreen");
        }

        #endregion

        public void LoadLevel(PackedScene level)
        {
            GD.Print($"[SceneManager] Loading level {level.ResourcePath}...");

            _loadingScreen.Show();

            GetTree().ChangeSceneTo(level);

            _loadingScreen.Hide();
        }
    }
}

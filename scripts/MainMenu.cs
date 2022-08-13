using Godot;

namespace pdxpartyparrot.ssjAug2022
{
    public class MainMenu : Node
    {
        [Export]
        private PackedScene _initialLevelScene;

        private SceneManager _sceneManager;

        #region Godot Lifecycle

        public override void _Ready()
        {
            _sceneManager = GetNode<SceneManager>("/root/SceneManager");
        }

        #endregion

        #region Signals

        private void _on_Play_pressed()
        {
            _sceneManager.LoadLevel(_initialLevelScene);
        }

        #endregion
    }
}

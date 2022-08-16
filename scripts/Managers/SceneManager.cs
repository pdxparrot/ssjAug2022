using Godot;

using System;
using System.Threading.Tasks;

using pdxpartyparrot.ssjAug2022.Util;

namespace pdxpartyparrot.ssjAug2022.Managers
{
    // TODO: we should thread loading the resource if the number of stages is high
    public class SceneManager : SingletonNode<SceneManager>
    {
        [Export]
        private PackedScene _mainMenuScene;

        [Export]
        private PackedScene _initialLevelScene;

        private Node _currentScene;

        #region Godot Lifecycle

        public override void _Ready()
        {
            base._Ready();

            LoadingScreen.Instance.Hide();

            CallDeferred("LoadMainMenu");
        }

        #endregion

        private void SetCurrentScene(PackedScene scene)
        {
            _currentScene = scene.Instance();
            _currentScene.Name = "Level";
            GetTree().Root.AddChild(_currentScene);
        }

        private void UpdateProgress(float progress)
        {
            GD.Print($"[SceneManager] {progress * 100.0}%");
        }

        private void ShowError(Error err)
        {
            GD.Print($"[SceneManager] Error loading level: {err}");
        }

        private void LoadMainMenu()
        {
            new Task(async () => await LoadMainMenuAsync().ConfigureAwait(false)).RunSynchronously();
        }

        public async Task LoadMainMenuAsync()
        {
            await LoadLevelAsync(_mainMenuScene).ConfigureAwait(false);
        }

        public async Task LoadInitialLevelAsync(Action onSuccess = null)
        {
            await LoadLevelAsync(_initialLevelScene, onSuccess).ConfigureAwait(false);
        }

        public async Task LoadLevelAsync(PackedScene level, Action onSuccess = null)
        {
            GD.Print($"[SceneManager] Loading level {level.ResourcePath}...");

            if(IsInstanceValid(_currentScene)) {
                _currentScene.QueueFree();
            }

            LoadingScreen.Instance.Show();

            await ResourceManager.Instance.LoadResourceAsync(level.ResourcePath,
                (owner, args) => {
                    SetCurrentScene((PackedScene)args.Resource);

                    onSuccess?.Invoke();
                },
                (owner, args) => ShowError(args.Error),
                (owner, args) => UpdateProgress(args.Progress)
            ).ConfigureAwait(false);
        }
    }
}

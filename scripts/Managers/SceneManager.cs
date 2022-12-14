using Godot;

using System;
using System.Threading.Tasks;

using pdxpartyparrot.ssjAug2022.Util;

namespace pdxpartyparrot.ssjAug2022.Managers
{
    // TODO: rename this LevelManager
    // TODO: we should thread loading the resource if the number of stages is high
    public class SceneManager : SingletonNode<SceneManager>
    {
        [Export]
        private PackedScene _loadingScreenScene;

        public LoadingScreen LoadingScreen { get; private set; }

        [Export]
        private PackedScene _mainMenuScene;

        [Export]
        private PackedScene _initialLevelScene;

        private Node _currentScene;

        #region Godot Lifecycle

        public override void _Ready()
        {
            base._Ready();

            CreateLoadingScreen();
        }

        #endregion

        private void CreateLoadingScreen()
        {
            if(IsInstanceValid(LoadingScreen)) {
                GD.PushWarning("[SceneManager] Re-creating Loading Screen ...");

                LoadingScreen.QueueFree();
            }

            LoadingScreen = (LoadingScreen)_loadingScreenScene.Instance();
            LoadingScreen.Name = "Loading Screen";
        }

        public void ShowLoadingScreen()
        {
            GD.Print("[SceneManager] Showing loading screen...");
            AddChild(LoadingScreen);
        }

        public void HideLoadingScreen()
        {
            GD.Print("[SceneManager] Hiding loading screen...");
            RemoveChild(LoadingScreen);
        }

        private void SetCurrentScene(PackedScene scene)
        {
            if(IsInstanceValid(_currentScene)) {
                GD.PushWarning("[SceneManager] Overwriting valid scene pointer!");
            }

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

        public async Task LoadMainMenuAsync()
        {
            GD.Print("[SceneManager] Loading main menu ...");
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

            ShowLoadingScreen();

            await ResourceManager.Instance.LoadResourceAsync(level.ResourcePath,
                (owner, args) => {
                    SetCurrentScene((PackedScene)args.Resource);

                    onSuccess?.Invoke();

                    HideLoadingScreen();
                },
                (owner, args) => ShowError(args.Error),
                (owner, args) => UpdateProgress(args.Progress)
            ).ConfigureAwait(false);
        }
    }
}

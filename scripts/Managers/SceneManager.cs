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
        private ulong maxLoadMs = 100;

        [Export]
        private PackedScene _mainMenuScene;

        [Export]
        private PackedScene _initialLevelScene;

        private Node _currentScene;

        private ResourceInteractiveLoader _loader;

        private bool _wait = false;

        private Action _onSuccess;

        #region Godot Lifecycle

        public override void _Ready()
        {
            base._Ready();

            LoadingScreen.Instance.Hide();

            CallDeferred("LoadMainMenu");
        }

        public override void _Process(float delta)
        {
            // don't run if we aren't loading anything
            if(_loader == null) {
                SetProcess(false);
                return;
            }

            // wait a frame for the loading screen to show
            if(_wait) {
                _wait = false;
                return;
            }

            ulong now = OS.GetTicksMsec();
            while(OS.GetTicksMsec() < now + maxLoadMs) {
                var err = _loader.Poll();
                if(err == Error.FileEof) {
                    // finished loading
                    UpdateProgress(1.0f);

                    var sceneResource = _loader.GetResource();
                    _loader.Dispose();
                    _loader = null;

                    SetCurrentScene((PackedScene)sceneResource);

                    _onSuccess?.Invoke();
                    _onSuccess = null;
                    break;
                } else if(err == Error.Ok) {
                    // still loading
                    UpdateProgress(_loader.GetStage() / (float)_loader.GetStageCount());
                } else {
                    _loader.Dispose();
                    _loader = null;
                    _onSuccess = null;

                    ShowError(err);
                    return;
                }
            }
        }

        #endregion

        private void SetCurrentScene(PackedScene scene)
        {
            _currentScene = scene.Instance();
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

using Godot;

using System;

using pdxpartyparrot.ssjAug2022.Util;

namespace pdxpartyparrot.ssjAug2022.Managers
{
    // TODO: we should thread loading the resource if the number of stages is high
    public class SceneManager : Singleton<SceneManager>
    {
        [Export]
        private ulong maxLoadMs = 100;

        private LoadingScreen _loadingScreen;

        private Node _currentScene;

        private ResourceInteractiveLoader _loader;

        private bool _wait = false;

        private Action _onSuccess;

        #region Godot Lifecycle

        public override void _Ready()
        {
            base._Ready();

            _loadingScreen = GetNode<LoadingScreen>("/root/LoadingScreen");
            _loadingScreen.Hide();

            GetCurrentScene();
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
                    _loader = null;

                    SetCurrentScene((PackedScene)sceneResource);

                    _onSuccess?.Invoke();
                    _onSuccess = null;
                    break;
                } else if(err == Error.Ok) {
                    // still loading
                    UpdateProgress(_loader.GetStage() / (float)_loader.GetStageCount());
                } else {
                    _loader = null;
                    _onSuccess = null;

                    ShowError();
                    return;
                }
            }
        }

        #endregion

        private void GetCurrentScene()
        {
            var root = GetTree().Root;
            _currentScene = root.GetChild(root.GetChildCount() - 1);
        }

        private void SetCurrentScene(PackedScene scene)
        {
            _currentScene = scene.Instance();
            GetTree().Root.AddChild(_currentScene);
        }

        private void UpdateProgress(float progress)
        {
            GD.Print($"[SceneManager] {progress * 100.0}%");
        }

        private void ShowError()
        {
            GD.Print($"[SceneManager] Error loading level!");
        }

        public void LoadLevel(PackedScene level, Action onSuccess = null)
        {
            _onSuccess = onSuccess;

            GD.Print($"[SceneManager] Loading level {level.ResourcePath}...");

            _loader = ResourceLoader.LoadInteractive(level.ResourcePath);
            GD.Print($"[SceneManager] {_loader.GetStageCount()} stage(s)");

            SetProcess(true);

            _currentScene.QueueFree();

            _loadingScreen.Show();
            UpdateProgress(0.0f);

            _wait = true;
        }
    }
}

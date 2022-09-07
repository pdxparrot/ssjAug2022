using Godot;

using System;
using System.Collections.Generic;
using System.Globalization;

using pdxpartyparrot.ssjAug2022.Util;

namespace pdxpartyparrot.ssjAug2022.Managers
{
    public sealed class PartyParrotManager : SingletonNode<PartyParrotManager>
    {
        #region Events

        public event EventHandler<EventArgs> PauseEvent;

        #endregion

        // initializes the default cultures to be invariant
        // https://twitter.com/OwenGoss/status/1413576850715156484
        private static void InitCulture()
        {
            GD.Print("[Engine] Setting invariant culture");

            // TODO: is this actually safe?
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
        }

        // TODO: this should be network sync'd and server-authoritative
        #region Game State

        [Export]
        private ulong _randomSeed;

        public ulong RandomSeed => _randomSeed;

        #region Godot Feature Helpers

        public bool IsEditor => OS.HasFeature("editor");

        public bool IsHeadless => OS.HasFeature("Server");

        public bool IsDebug => OS.HasFeature("debug");

        public bool IsRelease => OS.HasFeature("release");

        #endregion

        public bool IsFullscreen
        {
            get => OS.WindowFullscreen;
            set => OS.WindowFullscreen = value;
        }

        private bool _isPaused;

        public bool IsPaused
        {
            get => _isPaused;
            set
            {
                bool wasPaused = _isPaused;
                _isPaused = value;

                if(wasPaused != _isPaused) {
                    GD.Print($"[Engine] Pause: {_isPaused}");

                    PauseEvent?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        #endregion

        public RandomNumberGenerator Random { get; private set; }

        private readonly List<Func<bool>> _quitHandlers = new List<Func<bool>>();

        #region Godot Lifecycle

        public override void _Ready()
        {
            base._Ready();

            string build = IsRelease ? "release" : IsDebug ? "debug" : "unknown";
            GD.Print($"Party Parrot Engine ({build} build) starting up...");
            GD.Print($"Godot version: {Engine.GetVersionInfo()["string"]}");

            if(IsEditor) {
                GD.Print("Detected editor run");
            }

            if(IsHeadless) {
                GD.Print("Detected headless run");
            }

            InitCulture();

            GetTree().AutoAcceptQuit = false;

            SetRandomSeed(RandomSeed);

            DumpInfo();
        }

        public override void _Notification(int what)
        {
            if(what == MainLoop.NotificationWmQuitRequest) {
                GD.Print("[Engine] Received quit notification, running handlers ...");

                // TODO: what if a handler needs to prompt the user?
                foreach(var handler in _quitHandlers) {
                    if(!handler()) {
                        return;
                    }
                }

                GD.Print("[Engine] Quitting!");
                GetTree().Quit();
            }
        }

        #endregion

        private void DumpInfo()
        {
            GD.Print($"[Engine] OS: {OS.GetName()}");
            GD.Print($"[Engine] Multithreading: {OS.CanUseThreads()}");

            GD.Print($"[Engine] Target FPS: {Engine.TargetFps}");
            GD.Print($"[Engine] Vsync: {OS.VsyncEnabled}");

            GD.Print($"[Engine] Physics FPS: {Engine.IterationsPerSecond}");

            float gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
            var gravityVector = (Vector3)ProjectSettings.GetSetting("physics/3d/default_gravity_vector");
            GD.Print($"[Engine] Gravity: {gravityVector * gravity}");
        }

        public void RegisterQuitHandler(Func<bool> handler)
        {
            _quitHandlers.Add(handler);
        }

        public void SafeQuit()
        {
            GetTree().Notification(MainLoop.NotificationWmQuitRequest);
        }

        public void SetRandomSeed(ulong seed)
        {
            Random = new RandomNumberGenerator();

            _randomSeed = seed;
            if(RandomSeed > 0) {
                GD.Print($"[Engine] Seeding RNG {RandomSeed}...");
                Random.Seed = RandomSeed;
            } else {
                GD.Print($"[Engine] Randomizing...");
                Random.Randomize();
            }
        }

        public void TogglePause()
        {
            IsPaused = !IsPaused;
        }
    }
}

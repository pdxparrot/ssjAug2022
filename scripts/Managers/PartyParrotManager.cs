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

            InitCulture();

            GetTree().AutoAcceptQuit = false;

            // TODO: is this the best place for this log?
            GD.Print("Party Parrot Engine starting up...");
            GD.Print($"Godot version: {Engine.GetVersionInfo()["string"]}");

            float gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
            var gravityVector = (Vector3)ProjectSettings.GetSetting("physics/3d/default_gravity_vector");
            GD.Print($"Gravity: {gravityVector * gravity}");

            SetRandomSeed(RandomSeed);
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

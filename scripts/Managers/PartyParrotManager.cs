using Godot;

using System;
using System.Globalization;

namespace pdxpartyparrot.ssjAug2022.Managers
{
    public sealed class PartyParrotManager : Node
    {
        #region Events

        public event EventHandler<EventArgs> PauseEvent;

        #endregion

        // initializes the default cultures to be invariant
        // https://twitter.com/OwenGoss/status/1413576850715156484
        public static void InitCulture()
        {
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

        #region Godot Lifecycle

        public override void _Ready()
        {
            // TODO: is this the best place for this log?
            GD.Print("Party Parrot Engine starting up...");
            GD.Print($"Godot version: {Engine.GetVersionInfo()["string"]}");

            float gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
            var gravityVector = (Vector3)ProjectSettings.GetSetting("physics/3d/default_gravity_vector");
            GD.Print($"Gravity: {gravityVector * gravity}");

            SetRandomSeed(RandomSeed);
        }

        #endregion

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

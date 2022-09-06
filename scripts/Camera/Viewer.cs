using Godot;

using System;

using pdxpartyparrot.ssjAug2022.Managers;
using pdxpartyparrot.ssjAug2022.Util;

namespace pdxpartyparrot.ssjAug2022.Camera
{
    // this goes on the viewer's pivot container
    // not the camera itself
    public abstract class Viewer : Spatial
    {
        private struct ShakeSettings
        {
            public float amount;

            public float power;

            public uint noise_y;
        }

        public Godot.Camera Camera { get; private set; }

        private ShakeSettings _shakeSettings;

        private OpenSimplexNoise _noise;

        #region Godot Lifecycle

        public override void _Ready()
        {
            Camera = GetNode<Godot.Camera>("Camera");

            _noise = new OpenSimplexNoise {
                Seed = PartyParrotManager.Instance.Random.Next(),
                Octaves = 2,
                Period = 4,
            };
        }

        public override void _Process(float delta)
        {
            if(_shakeSettings.amount > 0.0f) {
                // TODO: fixed rate of decay here isn't great
                _shakeSettings.amount = Math.Max(_shakeSettings.amount - 0.4f * delta, 0.0f);

                DoShake();
            }
        }

        #endregion

        #region Events

        public virtual void OnRelease()
        {
        }

        #endregion

        // TODO: this should take a duration
        // from which we can calculate the rate of decay
        public void Shake(float amount, float power = 2.0f)
        {
            _shakeSettings.amount = amount.Clamp(0.0f, 1.0f);
            _shakeSettings.power = power;
            _shakeSettings.noise_y = 0;
        }

        private void DoShake()
        {
            float amount = (float)Math.Pow(_shakeSettings.amount, _shakeSettings.power);

            _shakeSettings.noise_y += 1;

            var rotation = Camera.Rotation;
            rotation.z = 0.1f * amount * _noise.GetNoise2d(_noise.Seed, _shakeSettings.noise_y);
            Camera.Rotation = rotation;

            Camera.HOffset = 100.0f * amount * _noise.GetNoise2d(_noise.Seed * 2, _shakeSettings.noise_y);
            Camera.VOffset = 75.0f * amount * _noise.GetNoise2d(_noise.Seed * 3, _shakeSettings.noise_y);
        }
    }
}

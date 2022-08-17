using Godot;

using System;

using pdxpartyparrot.ssjAug2022.Managers;
using pdxpartyparrot.ssjAug2022.Player;

namespace pdxpartyparrot.ssjAug2022.World
{
    public class SpawnPoint : Spatial
    {
        [Export]
        private string[] _tags = new string[0];

        public string[] Tags => _tags;

        private Godot.Object _owner;

        private Action _onRelease;

        #region Godot Lifecycle

        public override void _EnterTree()
        {
            Register();
        }

        public override void _ExitTree()
        {
            Release();
            UnRegister();
        }

        #endregion

        private void Register()
        {
            if(SpawnManager.HasInstance) {
                SpawnManager.Instance.RegisterSpawnPoint(this);
            }
        }

        private void UnRegister()
        {
            if(SpawnManager.HasInstance) {
                SpawnManager.Instance.UnregisterSpawnPoint(this);
            }
        }

        protected void InitSpatial(Spatial spatial)
        {
            spatial.Transform = Transform;
        }

        #region Spawn

        public Spatial SpawnFromScene(PackedScene scene, string name)
        {
            var spawned = (Spatial)scene.Instance();
            spawned.Name = name;

            InitSpatial(spawned);

            return spawned;
        }

        public SimplePlayer SpawnPlayer(PackedScene playerScene, string name)
        {
            var player = (SimplePlayer)SpawnFromScene(playerScene, name);

            return player;
        }

        public void ReSpawn(Spatial spatial)
        {
            InitSpatial(spatial);
        }

        #endregion

        #region Acquire

        public bool Acquire(Godot.Object owner, Action onRelease = null, bool force = false)
        {
            if(!force && null != _owner) {
                return false;
            }

            Release();

            _owner = owner;
            _onRelease = onRelease;

            UnRegister();

            return true;
        }

        public void Release()
        {
            if(null == _owner) {
                return;
            }

            _onRelease?.Invoke();
            _owner = null;

            Register();
        }

        #endregion
    }
}

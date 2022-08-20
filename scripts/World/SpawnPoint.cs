using Godot;

using System;

using pdxpartyparrot.ssjAug2022.Managers;
using pdxpartyparrot.ssjAug2022.NPCs;
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

        public T SpawnFromScene<T>(PackedScene scene, string name) where T : Spatial
        {
            var spawned = (T)scene.Instance();
            spawned.Name = name;

            InitSpatial(spawned);

            return spawned;
        }

        #region Players

        public SimplePlayer SpawnPlayer(PackedScene playerScene, string name)
        {
            var player = SpawnFromScene<SimplePlayer>(playerScene, name);

            // have to temporarily add the player so _Ready() is called
            AddChild(player);
            player.OnSpawn(this);
            RemoveChild(player);

            return player;
        }

        public void ReSpawnPlayer(SimplePlayer player)
        {
            InitSpatial(player);

            // have to temporarily add the player so _Ready() is called
            AddChild(player);
            player.OnReSpawn(this);
            RemoveChild(player);
        }

        #endregion

        #region NPCs

        public SimpleNPC SpawnNPC(PackedScene npcScene, string name)
        {
            var npc = SpawnFromScene<SimpleNPC>(npcScene, name);

            // have to temporarily add the npc so _Ready() is called
            AddChild(npc);
            npc.OnSpawn(this);
            RemoveChild(npc);

            return npc;
        }

        public void ReSpawnNPC(SimpleNPC npc)
        {
            InitSpatial(npc);

            // have to temporarily add the npc so _Ready() is called
            AddChild(npc);
            npc.OnReSpawn(this);
            RemoveChild(npc);
        }

        #endregion

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

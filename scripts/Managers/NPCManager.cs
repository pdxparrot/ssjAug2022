using Godot;

using System.Collections.Generic;

using pdxpartyparrot.ssjAug2022.NPCs;
using pdxpartyparrot.ssjAug2022.Util;
using pdxpartyparrot.ssjAug2022.World;

namespace pdxpartyparrot.ssjAug2022.Managers
{
    public class NPCManager : SingletonNode<NPCManager>
    {
        private readonly HashSet<SimpleNPC> _npcs = new HashSet<SimpleNPC>();

        public IReadOnlyCollection<SimpleNPC> NPCs => _npcs;

        public int NPCCount => NPCs.Count;

        public void RegisterNPC(SimpleNPC npc)
        {
            if(_npcs.Add(npc)) {
                GD.Print($"[NPCManager] Registered NPC {npc.Name}");
            }

            AddChild(npc);
        }

        public void UnRegisterNPC(SimpleNPC npc)
        {
            if(_npcs.Remove(npc)) {
                GD.Print($"[NPCManager] Unregistered NPC {npc.Name}");
            }

            RemoveChild(npc);
        }

        public void ReSpawnNPC(SimpleNPC npc, string tag)
        {
            GD.Print($"[NPCManager] Respawning NPC {npc.Name}");

            SpawnPoint spawnPoint = SpawnManager.Instance.GetSpawnPoint(tag);
            if(null == spawnPoint) {
                GD.PushError("Failed to get NPC spawnpoint!");
                return;
            }

            spawnPoint.ReSpawnNPC(npc);
        }

        public void DeSpawnNPC(SimpleNPC npc, bool destroy)
        {
            if(destroy) {
                GD.Print($"[NPCManager] Destroying npc {npc.Name}");

                npc.QueueFree();
            } else {
                GD.Print($"[NPCManager] Despawning npc {npc.Name}");
            }

            npc.OnDeSpawn();
        }

        public void DespawnAllNPCs(bool destroy)
        {
            GD.Print($"[NPCManager] Despawning {_npcs.Count} NPCs...");

            SimpleNPC[] npcs = new SimpleNPC[_npcs.Count];
            _npcs.CopyTo(npcs);

            foreach(var npc in npcs) {
                DeSpawnNPC(npc, destroy);
            }
        }
    }
}

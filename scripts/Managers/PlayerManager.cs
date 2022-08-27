using Godot;

using System.Collections.Generic;

using pdxpartyparrot.ssjAug2022.Player;
using pdxpartyparrot.ssjAug2022.Util;

namespace pdxpartyparrot.ssjAug2022.Managers
{
    public class PlayerManager : SingletonNode<PlayerManager>
    {
        [Export]
        private PackedScene _playerScene;

        private readonly Dictionary<int, SimplePlayer> _players = new Dictionary<int, SimplePlayer>();

        public IReadOnlyDictionary<int, SimplePlayer> Players => _players;

        public int PlayerCount => Players.Count;

        public SimplePlayer SpawnPlayer(int clientId)
        {
            GD.Print($"[PlayerManager] Spawning player {clientId}...");

            var spawnPoint = SpawnManager.Instance.GetPlayerSpawnPoint(clientId);
            if(null == spawnPoint) {
                GD.PushError("Failed to get player spawnpoint!");
                return null;
            }

            if(_players.TryGetValue(clientId, out SimplePlayer player)) {
                spawnPoint.ReSpawnPlayer(player);
            } else {
                player = spawnPoint.SpawnPlayer(_playerScene, $"Player {clientId}");
                player.ClientId = clientId;
                _players[clientId] = player;
            }

            AddChild(player);

            return player;
        }

        public void DeSpawnPlayer(SimplePlayer player)
        {
            GD.Print($"[PlayerManager] Despawning player {player.Name}");

            player.OnDeSpawn();

            RemoveChild(player);
        }

        public void DeSpawnPlayers()
        {
            if(PlayerCount < 1) {
                return;
            }

            foreach(var kvp in _players) {
                DeSpawnPlayer(kvp.Value);
            }
        }

        public void DestroyPlayer(SimplePlayer player, bool remove = true)
        {
            GD.Print($"[PlayerManager] Destroying player {player.Name}");

            if(remove) {
                _players.Remove(player.ClientId);
            }

            player.QueueFree();
        }

        public void DestroyPlayers()
        {
            if(PlayerCount < 1) {
                return;
            }

            foreach(var kvp in _players) {
                DestroyPlayer(kvp.Value, false);
            }

            _players.Clear();
        }
    }
}

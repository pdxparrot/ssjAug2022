using Godot;

using System.Collections.Generic;

using pdxpartyparrot.ssjAug2022.Player;
using pdxpartyparrot.ssjAug2022.Util;
using pdxpartyparrot.ssjAug2022.World;

namespace pdxpartyparrot.ssjAug2022.Managers
{
    public class PlayerManager : SingletonNode<PlayerManager>
    {
        [Export]
        private PackedScene _playerScene;

        private readonly HashSet<SimplePlayer> _players = new HashSet<SimplePlayer>();

        public IReadOnlyCollection<SimplePlayer> Players => _players;

        public int PlayerCount => Players.Count;

        public void SpawnPlayer(int clientId)
        {
            GD.Print($"Spawning player {clientId}...");

            SpawnPoint spawnPoint = SpawnManager.Instance.GetPlayerSpawnPoint(clientId);
            if(null == spawnPoint) {
                GD.PushError("Failed to get player spawnpoint!");
                return;
            }

            var player = spawnPoint.SpawnPlayer(_playerScene, $"Player {clientId}");
            player.Show();

            _players.Add(player);
        }

        public void DespawnPlayer(SimplePlayer player)
        {
            //GD.Print($"Despawning player {player.Id}");
            GD.Print("Despawning player");

            player.Hide();
        }

        public void DespawnPlayers()
        {
            if(PlayerCount < 1) {
                return;
            }

            foreach(var player in _players) {
                DespawnPlayer(player);
            }
        }

        public void DestroyPlayer(SimplePlayer player, bool remove = true)
        {
            //GD.Print($"Destroying player {player.Id}");
            GD.Print("Destroying player");

            player.QueueFree();

            if(remove) {
                _players.Remove(player);
            }
        }

        public void DestroyPlayers()
        {
            if(PlayerCount < 1) {
                return;
            }

            foreach(var player in _players) {
                DestroyPlayer(player, false);
            }

            _players.Clear();
        }
    }
}

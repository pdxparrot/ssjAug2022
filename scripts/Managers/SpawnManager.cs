using Godot;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using pdxpartyparrot.ssjAug2022.Collections;
using pdxpartyparrot.ssjAug2022.Util;
using pdxpartyparrot.ssjAug2022.World;

namespace pdxpartyparrot.ssjAug2022.Managers
{
    public class SpawnManager : SingletonNode<SpawnManager>
    {
        public enum SpawnMethod
        {
            Random,

            RoundRobin
        }

        private sealed class SpawnPointSet
        {
            public List<SpawnPoint> SpawnPoints { get; } = new List<SpawnPoint>();

            private int _nextRoundRobinIndex;

            public void SeedRoundRobin()
            {
                _nextRoundRobinIndex = PartyParrotManager.Instance.Random.Next(SpawnPoints.Count);
            }

            public SpawnPoint GetSpawnPoint(SpawnMethod spawnMethod)
            {
                if(SpawnPoints.Count < 1) {
                    return null;
                }

                // just in case
                if(_nextRoundRobinIndex >= SpawnPoints.Count) {
                    AdvanceRoundRobin();
                }

                switch(spawnMethod) {
                case SpawnMethod.RoundRobin:
                    SpawnPoint spawnPoint = SpawnPoints[_nextRoundRobinIndex];
                    AdvanceRoundRobin();
                    return spawnPoint;
                case SpawnMethod.Random:
                    return PartyParrotManager.Instance.Random.GetRandomEntry(SpawnPoints);
                default:
                    GD.PushWarning($"Unsupported spawn method {spawnMethod}, using Random");
                    return PartyParrotManager.Instance.Random.GetRandomEntry(SpawnPoints);
                }
            }

            public SpawnPoint GetNearestSpawnPoint(Vector3 position)
            {
                if(SpawnPoints.Count < 1) {
                    return null;
                }
                return SpawnPoints.NearestManhattan(position, out _);
            }

            private void AdvanceRoundRobin()
            {
                _nextRoundRobinIndex = (_nextRoundRobinIndex + 1) % SpawnPoints.Count;
            }
        }

        [Export]
        private PackedScene _playerScene;

        [Export]
        private string[] _playerSpawnPointTags = new string[0];

        [Export]
        private Dictionary<string, SpawnMethod> _spawnTypes = new Dictionary<string, SpawnMethod>();

        private readonly Dictionary<string, SpawnPointSet> _spawnPoints = new Dictionary<string, SpawnPointSet>();

        #region Godot Lifecycle

        public override void _Ready()
        {
            base._Ready();

            Debug.Assert(_playerSpawnPointTags.Length > 0);
        }

        #endregion

        #region Registration

        public virtual void RegisterSpawnPoint(SpawnPoint spawnPoint)
        {
            GD.Print($"[SpawnManager] Registering spawnpoint '{spawnPoint.Name}' with tags: {string.Join(",", spawnPoint.Tags)}");

            foreach(string tag in spawnPoint.Tags) {
                _spawnPoints.GetOrAdd(tag).SpawnPoints.Add(spawnPoint);
            }
        }

        public virtual void UnregisterSpawnPoint(SpawnPoint spawnPoint)
        {
            GD.Print($"[SpawnManager] Unregistering spawnpoint '{spawnPoint.Name}'");

            foreach(string tag in spawnPoint.Tags) {
                if(_spawnPoints.TryGetValue(tag, out var spawnPoints)) {
                    spawnPoints.SpawnPoints.Remove(spawnPoint);
                }
            }
        }

        #endregion

        public void Initialize()
        {
            GD.Print("[SpawnManager] Seeding spawn points...");
            foreach(var kvp in _spawnPoints) {
                kvp.Value.SeedRoundRobin();
            }
        }

        public int SpawnPointCount(string tag)
        {
            if(!_spawnPoints.TryGetValue(tag, out var spawnPoints)) {
                GD.PushWarning($"No spawn points with tag '{tag}' registered on spawn, are there any in the scene?");
                return 0;
            }

            return spawnPoints.SpawnPoints.Count;
        }

        public IReadOnlyCollection<SpawnPoint> GetSpawnPoints(string tag)
        {
            if(!_spawnPoints.TryGetValue(tag, out var spawnPoints)) {
                GD.PushWarning($"No spawn points with tag '{tag}' registered on spawn, are there any in the scene?");
                return new List<SpawnPoint>();
            }
            return spawnPoints.SpawnPoints;
        }

        public SpawnPoint GetSpawnPoint(string tag)
        {
            if(!_spawnPoints.TryGetValue(tag, out var spawnPoints)) {
                GD.PushWarning($"No spawn points with tag '{tag}' registered on spawn, are there any in the scene?");
                return null;
            }

            var spawnMethod = _spawnTypes.GetValueOrDefault(tag);
            return spawnPoints.GetSpawnPoint(spawnMethod);
        }

        // gets a random spawnpoint regardless of how the spawnpoints are configured
        public SpawnPoint GetRandomSpawnPoint(string tag)
        {
            if(!_spawnPoints.TryGetValue(tag, out var spawnPoints)) {
                GD.PushWarning($"No spawn points with tag '{tag}' registered on spawn, are there any in the scene?");
                return null;
            }
            return spawnPoints.GetSpawnPoint(SpawnMethod.Random);
        }

        // gets the spawnpoint nearest the given position
        public SpawnPoint GetNearestSpawnPoint(string tag, Vector3 position)
        {
            if(!_spawnPoints.TryGetValue(tag, out var spawnPoints)) {
                GD.PushWarning($"No spawn points with tag '{tag}' registered on spawn, are there any in the scene?");
                return null;
            }
            return spawnPoints.GetNearestSpawnPoint(position);
        }

        #region Player Spawnpoints

        public SpawnPoint GetPlayerSpawnPoint(ulong clientId)
        {
            int spawnPointIdx = Mathf.Clamp((int)clientId, 0, _playerSpawnPointTags.Length - 1);
            return GetSpawnPoint(_playerSpawnPointTags.ElementAt(spawnPointIdx));
        }

        // gets a random player spawnpoint regardless of how the spawnpoints are configured
        public SpawnPoint GetRandomPlayerSpawnPoint(ulong clientId)
        {
            int spawnPointIdx = Mathf.Clamp((int)clientId, 0, _playerSpawnPointTags.Length - 1);
            return GetRandomSpawnPoint(_playerSpawnPointTags.ElementAt(spawnPointIdx));
        }

        // gets the player spawnpoint nearest the given position
        public SpawnPoint GetNearestPlayerSpawnPoint(ulong clientId, Vector3 position)
        {
            int spawnPointIdx = Mathf.Clamp((int)clientId, 0, _playerSpawnPointTags.Length - 1);
            return GetNearestSpawnPoint(_playerSpawnPointTags.ElementAt(spawnPointIdx), position);
        }

        public SpawnPoint GetPlayerSpawnPoint(string tag)
        {
            return GetSpawnPoint(tag);
        }

        // gets a random player spawnpoint regardless of how the spawnpoints are configured
        public SpawnPoint GetRandomPlayerSpawnPoint(string tag)
        {
            return GetRandomSpawnPoint(tag);
        }

        // gets the player spawnpoint nearest the given position
        public SpawnPoint GetNearestPlayerSpawnPoint(string tag, Vector3 position)
        {
            return GetNearestSpawnPoint(tag, position);
        }

        #endregion
    }
}

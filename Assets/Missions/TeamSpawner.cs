using UnityEngine;
using System.Collections;

public class TeamSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Team
    {
        [SerializeField]
        private string name;

        public Ship shipPrefab;

        [SerializeField]
        private Transform[] spawnPoints;

        [HideInInspector]
        [SerializeField]
        private Ship[] spawnedShips;

        public string Name { get { return name; } }
        public Transform[] SpawnPoints { get { return spawnPoints; } }
        public Ship[] SpawnedShips { get { return spawnedShips; } }

        public void SpawnAll()
        {
            if (spawnedShips != null && spawnedShips.Length != 0)
            {
                throw new UnityException("already spawned this team once");
            }

            spawnedShips = new Ship[spawnPoints.Length];

            for (int spawn = 0; spawn < spawnPoints.Length; ++spawn)
            {
                var spawnPoint = spawnPoints[spawn];

                var ship = (Ship)Instantiate(shipPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);

                var targetable = ship.GetComponent<Targetable>();
                if (targetable)
                {
                    targetable.Faction = Name;
                }

                spawnedShips[spawn] = ship;
            }
        }
    }

    [SerializeField]
    private Team[] teams;

    public Team[] Teams { get { return teams; } }

    void OnBeginMission()
    {
        var mission = MissionManager.Instance.Mission;
        
        foreach (var team in teams)
        {
            team.SpawnAll();
        }

        bool first = true;

        foreach (var team in Teams)
        {
            foreach (var ship in team.SpawnedShips)
            {
                if (first)
                {
                    var localPlayer = ship.gameObject.AddComponent<PlayerShip>();
                    localPlayer.MakeLocal();

                    first = false;
                }
                else
                {
                    ship.gameObject.AddComponent<AICaptain>();
                }
            }
        }
    }
}

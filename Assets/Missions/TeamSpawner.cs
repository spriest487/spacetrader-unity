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

        public string Name { get { return name; } }
        public Transform[] SpawnPoints { get { return spawnPoints; } }
    }

    [SerializeField]
    private Team[] teams;

    void OnBeginMission()
    {
        var mission = MissionManager.Instance.Mission;

        bool first = true;

        foreach (var team in teams)
        {
            foreach (var spawnPoint in team.SpawnPoints)
            {
                var ship = (Ship) Instantiate(team.shipPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);

                if (first)
                {
                    var playerShip = ship.gameObject.AddComponent<PlayerShip>();
                    playerShip.MakeLocal();
                    first = false;
                }

                var targetable = ship.GetComponent<Targetable>();
                if (targetable)
                {
                    targetable.Faction = team.Name;
                }
            }
        }
    }
}

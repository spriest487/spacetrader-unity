using UnityEngine;
using System.Collections.Generic;

public class TeamSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Team
    {
        [SerializeField]
        private string name;

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
            var mission = MissionManager.Instance.Mission;

            if (spawnedShips != null && spawnedShips.Length != 0)
            {
                throw new UnityException("already spawned this team once");
            }

            /* find the team in the mission definition that matches the team name of 
             * this team in the spawner 
             */
            MissionDefinition.TeamDefinition missionTeam = null;
            foreach (var team in mission.Definition.Teams)
            {
                if (team.Name == Name)
                {
                    missionTeam = team;
                    break;
                }
            }

            if (missionTeam == null)
            {
                Debug.LogError("mission had no matching team for name " +Name);
                return;
            }

            var spawned = new List<Ship>();

            /* loop around slots, spawning players at each slot in turn until there are
             * no more players 
             */
            var slotsCount = missionTeam.Slots.Length;
            var spawnsCount = spawnPoints.Length;

            for (int slotIndex = 0; slotIndex < slotsCount; ++slotIndex)
            {
                var slot = missionTeam.Slots[slotIndex];
                var spawnPoint = spawnPoints[slotIndex % spawnsCount];

                var ship = slot.SpawnShip(spawnPoint.position, spawnPoint.rotation, missionTeam);

                spawned.Add(ship);
            }

            spawnedShips = spawned.ToArray();
        }
    }

    [SerializeField]
    private Team[] teams;

    /// <summary>
    /// spawn point list for each team
    /// </summary>
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
            var shipNum = 1;
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
                    ship.gameObject.AddComponent<WingmanCaptain>();
                }

                ship.name = "Team " + team.Name + " ship #" + shipNum++;
            }
        }
    }
}

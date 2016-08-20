#pragma warning disable 0649

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

        public void SpawnAll(MissionDefinition.TeamDefinition teamDefinition, ActiveTeam activeTeam)
        {
            if (spawnedShips != null && spawnedShips.Length != 0)
            {
                throw new UnityException("already spawned this team once");
            }
            
            var spawned = new List<Ship>();

            /* loop around slots, spawning players at each slot in turn until there are
             * no more players 
             */
            var slotsCount = teamDefinition.Slots.Length;

            var allSpawnPoints = new List<Transform>();
            foreach (Transform spawnRoot in spawnPoints)
            {
                foreach (Transform spawnPoint in spawnRoot)
                {
                    allSpawnPoints.Add(spawnPoint);
                }
            }

            Ship firstSpawned = null;

            var spawnsCount = allSpawnPoints.Count;

            for (int slotIndex = 0; slotIndex < slotsCount; ++slotIndex)
            {
                var slotDefinition = teamDefinition.Slots[slotIndex];
                var spawnPoint = allSpawnPoints[slotIndex % spawnsCount];

                var activeSlot = activeTeam.Slots[slotIndex];
                
                if (activeSlot.Status != SlotStatus.Closed || activeSlot.Status == SlotStatus.Open)
                {
                    var ship = slotDefinition.SpawnShip(spawnPoint.position, spawnPoint.rotation, teamDefinition);

                    //first spawned ship becomnes the leader of the fleet
                    if (firstSpawned == null)
                    {
                        firstSpawned = ship;
                    }
                    else
                    {
                        SpaceTraderConfig.FleetManager.AddToFleet(firstSpawned, ship);
                    }

                    switch(activeSlot.Status)
                    {
                        case SlotStatus.AI:
                            ship.gameObject.AddComponent<AICaptain>();
                            ship.gameObject.AddComponent<WingmanCaptain>();
                            break;
                        case SlotStatus.Human:
                            SpaceTraderConfig.LocalPlayer = ship.gameObject.AddComponent<PlayerShip>();
                            break;
                    }

                    spawned.Add(ship);
                }
            }

            spawnedShips = spawned.ToArray();
        }
    }

    [SerializeField]
    private Team[] teams;
    
    public Team[] Teams { get { return teams; } }

    private Team FindTeam(string name)
    {
        foreach (var team in teams)
        {
            if (team.Name == name)
            {
                return team;
            }
        }

        return null;
    }

    void OnBeginMission()
    {
        var mission = MissionManager.Instance.Mission;
        
        for (int team = 0; team < mission.Teams.Length; ++team)
        {
            var teamDefinition = mission.Definition.Teams[team];
            var activeTeam = mission.Teams[team];

            var spawnedTeam = FindTeam(teamDefinition.Name);

            if (spawnedTeam != null)
            {
                spawnedTeam.SpawnAll(teamDefinition, activeTeam);
            }
        }
    }
}

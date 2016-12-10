#pragma warning disable 0649

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TeamSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Team
    {
        [SerializeField]
        private string name;

        [SerializeField]
        private Transform[] spawnPoints;

        public string Name { get { return name; } }
        public Transform[] SpawnPoints { get { return spawnPoints; } }

        public void SpawnAll(MissionDefinition.TeamDefinition teamDefinition, ActiveTeam activeTeam)
        {
            /* loop around slots, spawning players at each slot in turn until there are
             * no more players
             */
            var slotsCount = teamDefinition.Slots.Count;

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

                    switch (activeSlot.Status)
                    {
                        case SlotStatus.AI:
                            SetupAIPlayer(ship);
                            break;
                        case SlotStatus.Human:
                            SetupHumanPlayer(ship, teamDefinition);
                            break;
                    }

                    activeSlot.SpawnedShip = ship;
                }
            }
        }

        private void SetupHumanPlayer(Ship ship, MissionDefinition.TeamDefinition teamDef)
        {
            var player = ship.gameObject.AddComponent<PlayerShip>();
            SpaceTraderConfig.LocalPlayer = player;
        }

        private void SetupAIPlayer(Ship ship)
        {
            ship.gameObject.AddComponent<WingmanCaptain>();
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

    private void Start()
    {
        MissionManager.Instance.OnPhaseChanged += OnPhaseChanged;
    }

    private void OnDestroy()
    {
        MissionManager.Instance.OnPhaseChanged -= OnPhaseChanged;
    }

    void OnPhaseChanged(MissionPhase phase)
    {
        var mission = MissionManager.Instance.Mission;

        switch (phase)
        {
            case MissionPhase.Active:
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
                break;
        }
    }
}

#pragma warning disable 0649

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TeamSpawnerTeam
{
    [SerializeField]
    private string name;

    [SerializeField]
    private Transform[] spawnPoints;

    public string Name { get { return name; } }
    public Transform[] SpawnPoints { get { return spawnPoints; } }

    public void SpawnAll(TeamDefinition teamDefinition,
        ActiveTeam activeTeam,
        string spawnTag)
    {
        /* loop around slots, spawning players at each slot in turn until there are
            * no more players
            */
        var slotsWithTag = teamDefinition.Slots
            .Where(s => s.MatchesSpawnTag(spawnTag))
            .ToList();

        var slotsCount = slotsWithTag.Count;

        var allSpawnPoints = new List<Transform>(slotsCount);

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
            var slotDefinition = slotsWithTag[slotIndex];
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
                    Universe.FleetManager.AddToFleet(firstSpawned, ship);
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

    private void SetupHumanPlayer(Ship ship, TeamDefinition teamDef)
    {
        var player = ship.gameObject.AddComponent<PlayerShip>();
        Universe.LocalPlayer = player;
    }

    private void SetupAIPlayer(Ship ship)
    {
        ship.gameObject.AddComponent<CombatAI>();
    }
}
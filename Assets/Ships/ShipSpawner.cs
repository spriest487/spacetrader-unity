#pragma warning disable 0649

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShipSpawner : MonoBehaviour
{
    [SerializeField]
    private ShipType shipType;

    [Header("Player Spawn")]

    [SerializeField]
    private bool makeLocalPlayer = false;

    [SerializeField]
    private int money = 1000;

    [Header("Crew")]

    [SerializeField]
    private List<CrewMember> passengers;

    [SerializeField]
    private CrewMember captain;

    [Header("Loadout")]

    [SerializeField]
    private ModulePreset modulePreset;

    [Header("Other spawners")]

    private ShipSpawner joinFleetOf;

    private Ship spawned;

    private void Start()
    {
        spawned = shipType.CreateShip(transform.position, transform.rotation);

        if (makeLocalPlayer)
        {
            Debug.Assert(!SpaceTraderConfig.LocalPlayer, "local player should not already be spawned");

            var player = spawned.gameObject.AddComponent<PlayerShip>();
            player.AddMoney(money);

            SpaceTraderConfig.LocalPlayer = player;
        }

        //crew
        var characters = SpaceTraderConfig.CrewConfiguration;

        passengers.Where(p => !!p)
            .Select(p => characters.NewCharacter(p))
            .ToList()
            .ForEach(p => p.Assign(spawned, CrewAssignment.Passenger));

        if (captain)
        {
            characters.NewCharacter(captain).Assign(spawned, CrewAssignment.Captain);
        }

        //modules
        if (modulePreset)
        {
            modulePreset.Apply(spawned);
        }
    }

    void Update()
    {
        if (spawned && joinFleetOf && joinFleetOf.spawned)
        {
            SpaceTraderConfig.FleetManager.AddToFleet(joinFleetOf.spawned, spawned);
        }

        Destroy(gameObject);
    }
}
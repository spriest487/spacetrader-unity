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
    private string dockedAt;

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

    [SerializeField]
    private ShipSpawner joinFleetOf;

    [SerializeField]
    private bool combatAI;

    [SerializeField]
    private Transform[] additionalChildren;
        
    private Ship spawned;

    private void Start()
    {
        spawned = shipType.CreateShip(transform.position, transform.rotation);

        if (!string.IsNullOrEmpty(dockedAt) && spawned.GetComponent<DockableObject>())
        {
            var dockedStation = GameObject.Find(dockedAt);
            if (dockedStation)
            {
                dockedStation.GetComponent<SpaceStation>()
                    .AddDockedShip(spawned.GetComponent<DockableObject>());
            }
        }

        //crew
        var characters = Universe.CrewConfiguration;

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

        /* do this last, because the local player observes stuff with ui transitions
         * etc which we probably don't want on player spawn */
        if (makeLocalPlayer)
        {
            Debug.Assert(!Universe.LocalPlayer, "local player should not already be spawned");
            var player = PlayerShip.MakePlayer(spawned);
            player.AddMoney(money);

            Universe.LocalPlayer = player;
        }

        if (combatAI)
        {
            spawned.gameObject.AddComponent<CombatAI>();
        }

        foreach (var child in additionalChildren)
        {
            Instantiate(child, spawned.transform, false);
        }
    }

    void LateUpdate()
    {
        if (spawned && joinFleetOf && joinFleetOf.spawned)
        {
            Universe.FleetManager.AddToFleet(joinFleetOf.spawned, spawned);
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (shipType && shipType.Prefab)
        {
            shipType.Prefab.gameObject.DrawPrefabWire(transform);
        }

        if (joinFleetOf)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, joinFleetOf.transform.position);
        }
    }
}
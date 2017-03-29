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

    [Header("Options")]

    [SerializeField]
    private bool spawnOnStart = true;

    [SerializeField]
    private bool keepAlive = false;

    [SerializeField]
    private ShipSpawner joinFleetOf;

    [SerializeField]
    private string joinFaction;

    [SerializeField]
    private bool combatAI = true;

    [SerializeField]
    private Transform[] additionalChildren;
        
    public Ship Spawned { get; private set; }

    private void Start()
    {
        if (spawnOnStart)
        {
            Spawn();
        }
    }

    public void Spawn()
    {
        Spawned = shipType.CreateShip(transform.position, transform.rotation);
        if (transform.parent)
        {
            Spawned.transform.SetParent(transform.parent, true);
        }

        if (!string.IsNullOrEmpty(dockedAt) && Spawned.GetComponent<DockableObject>())
        {
            var dockedStation = GameObject.Find(dockedAt);
            if (dockedStation)
            {
                dockedStation.GetComponent<SpaceStation>()
                    .AddDockedShip(Spawned.GetComponent<DockableObject>());
            }
        }

        //crew
        var characters = Universe.CrewConfiguration;

        passengers.Where(p => !!p)
            .Select(p => characters.NewCharacter(p))
            .ToList()
            .ForEach(p => p.Assign(Spawned, CrewAssignment.Passenger));

        if (captain)
        {
            characters.NewCharacter(captain).Assign(Spawned, CrewAssignment.Captain);
        }

        //modules
        if (modulePreset)
        {
            modulePreset.Apply(Spawned);
        }

        /* do this last, because the local player observes stuff with ui transitions
         * etc which we probably don't want on player spawn */
        if (makeLocalPlayer)
        {
            Debug.Assert(!Universe.LocalPlayer, "local player should not already be spawned");
            var player = PlayerShip.MakePlayer(Spawned);
            player.AddMoney(money);

            Universe.LocalPlayer = player;
        }

        if (!string.IsNullOrEmpty(joinFaction) && Spawned.Targetable)
        {
            Spawned.Targetable.Faction = joinFaction.Trim();
        }

        if (combatAI)
        {
            Spawned.gameObject.AddComponent<CombatAI>();
        }

        foreach (var child in additionalChildren)
        {
            Instantiate(child, Spawned.transform, false);
        }

        Spawned.name = name;
    }

    void LateUpdate()
    {
        if (Spawned && joinFleetOf && joinFleetOf.Spawned)
        {
            Universe.FleetManager.AddToFleet(joinFleetOf.Spawned, Spawned);
        }

        if (!keepAlive)
        {
            Destroy(gameObject);
        }
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
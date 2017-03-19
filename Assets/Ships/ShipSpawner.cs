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
    }

    void Update()
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
            var meshes = shipType.Prefab.GetComponentsInChildren<MeshFilter>();
            foreach (var meshFilter in meshes)
            {
                if (!meshFilter.sharedMesh)
                {
                    continue;
                }

                var meshXform = meshFilter.transform;
                var prefabRoot = meshXform.root;

                var pos = transform.position + meshXform.position - prefabRoot.position;
                var rot = transform.rotation * meshXform.rotation * Quaternion.Inverse(prefabRoot.rotation);

                var scale = transform.lossyScale;
                scale.x *= meshXform.lossyScale.x;
                scale.y *= meshXform.lossyScale.y;
                scale.z *= meshXform.lossyScale.z;
                
                Gizmos.DrawWireMesh(meshFilter.sharedMesh, pos, rot, scale);
            }
        }

        if (joinFleetOf)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, joinFleetOf.transform.position);
        }
    }
}
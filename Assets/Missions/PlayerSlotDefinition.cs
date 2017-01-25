#pragma warning disable 0649

using System;
using UnityEngine;

[Serializable]
public class PlayerSlot
{
    [SerializeField]
    private ShipType shipType;

    [SerializeField]
    private ModulePreset modulePreset;

    [SerializeField]
    private string name;

    [SerializeField]
    private string spawnTag;

    public ShipType ShipType { get { return shipType; } }
    public ModulePreset ModulePreset { get { return modulePreset; } }
    public string Name { get { return name; } }

    public Ship SpawnShip(Vector3 pos, Quaternion rot, TeamDefinition team)
    {
        var ship = ShipType.CreateShip(pos, rot);
        ship.name = Name;

        var targetable = ship.GetComponent<Targetable>();
        if (targetable)
        {
            targetable.Faction = team.Name;
        }

        if (modulePreset)
        {
            modulePreset.Apply(ship);
        }

        return ship;
    }

    public bool MatchesSpawnTag(string spawnTag)
    {
        if (string.IsNullOrEmpty(spawnTag))
        {
            return string.IsNullOrEmpty(this.spawnTag);
        }
        else
        {
            return string.Equals(spawnTag, this.spawnTag);
        }
    }
}
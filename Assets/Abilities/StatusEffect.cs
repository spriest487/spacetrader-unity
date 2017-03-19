using UnityEngine;
using System;

[Serializable]
public class StatusEffect
{
    public string Name { get; private set; }

    public ShipStats FlatStats { get; private set; }
    public ShipStats ProportionalStats { get; private set; }

    public float Expires { get; private set; }

    public StatusEffect(string name, 
        float expires, 
        ShipStats flatBonus = null, 
        ShipStats proportionalBonus = null)
    {
        Name = name;
        Expires = expires;
        FlatStats = flatBonus ?? new ShipStats();
        ProportionalStats = proportionalBonus ?? new ShipStats();
    }
}

using System;
using UnityEngine;

public class StatusEffect : ScriptableObject
{
    public UnityEngine.Object Source;

    public ShipStats FlatStats = new ShipStats();
    public ShipStats ProportionalStats = new ShipStats();

    public float Lifetime = 0;
    public bool Expires = false;
}

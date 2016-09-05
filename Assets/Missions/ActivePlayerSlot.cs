using UnityEngine;
using System;

[Serializable]
public class ActivePlayerSlot
{
    [SerializeField]
    private SlotStatus slotStatus;

    [SerializeField]
    private Ship spawnedShip;
    
    public SlotStatus Status
    {
        get { return slotStatus; }
        set { slotStatus = value; }
    }

    public Ship SpawnedShip
    {
        get { return spawnedShip; }
        set { spawnedShip = value; }
    }
}

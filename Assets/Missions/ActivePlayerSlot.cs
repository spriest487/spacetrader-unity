using UnityEngine;
using System;

[Serializable]
public class ActivePlayerSlot
{
    [SerializeField]
    private SlotStatus slotStatus;

    [SerializeField]
    private string humanPlayer;

    public SlotStatus Status
    {
        get { return slotStatus; }
        set
        {
            slotStatus = value;
            if (slotStatus != SlotStatus.Human)
            {
                humanPlayer = null;
            }
        }
    }

    public string HumanPlayer
    {
        get { return humanPlayer; }
        set { humanPlayer = value; }
    }
}

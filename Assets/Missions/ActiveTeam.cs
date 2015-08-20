using UnityEngine;
using System;

[Serializable]
public class ActiveTeam
{
    [SerializeField]
    private ActivePlayerSlot[] slots;

    public ActivePlayerSlot[] Slots { get { return slots; } }

    public ActiveTeam()
    {
    }

    public ActiveTeam(MissionDefinition.TeamDefinition team)
    {
        slots = new ActivePlayerSlot[team.Slots.Length];
        
        for (int slot = 0; slot < team.Slots.Length; ++slot)
        {
            var newSlot = new ActivePlayerSlot();
            slots[slot] = newSlot;
        }
    }
}

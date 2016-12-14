using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class ActiveTeam
{
    [SerializeField]
    private ActivePlayerSlot[] slots;

    [SerializeField]
    private TeamDefinition definition;

    public ActivePlayerSlot[] Slots
    {
        get { return slots; }
    }

    public TeamDefinition Definition
    {
        get { return definition; }
    }

    public ActiveTeam()
    {
    }

    public ActiveTeam(TeamDefinition team)
    {
        definition = team;

        slots = new ActivePlayerSlot[team.SlotCount];

        for (int slot = 0; slot < team.SlotCount; ++slot)
        {
            var newSlot = new ActivePlayerSlot();
            slots[slot] = newSlot;
        }
    }
}

using UnityEngine;
using System;

[Serializable]
public class ActiveTeam
{
    [SerializeField]
    private ActivePlayerSlot[] slots;

    [SerializeField]
    private MissionDefinition.TeamDefinition definition;

    public ActivePlayerSlot[] Slots
    {
        get { return slots; }
    }

    public MissionDefinition.TeamDefinition Definition
    {
        get { return definition; }
    }

    public ActiveTeam()
    {
    }

    public ActiveTeam(MissionDefinition.TeamDefinition team)
    {
        definition = team;

        slots = new ActivePlayerSlot[team.Slots.Count];
        
        for (int slot = 0; slot < team.Slots.Count; ++slot)
        {
            var newSlot = new ActivePlayerSlot();
            slots[slot] = newSlot;
        }
    }
}

#pragma warning disable 0649

using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TeamDefinition
{
    [SerializeField]
    private string name;

    [SerializeField]
    private List<PlayerSlot> slots;

    [SerializeField]
    private List<Quest> quests;

    public string Name { get { return name; } }
    public IEnumerable<PlayerSlot> Slots { get { return slots; } }
    public IEnumerable<Quest> Quests { get { return quests; } }
    public int SlotCount { get { return slots.Count; } }

    public PlayerSlot GetSlot(int slot)
    {
        return slots[slot];
    }

    public Quest GetQuest(int quest)
    {
        return quests[quest];
    }
}
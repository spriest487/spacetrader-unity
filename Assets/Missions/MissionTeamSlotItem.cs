using UnityEngine;
using UnityEngine.UI;
using System;

public class MissionTeamSlotItem : MonoBehaviour
{
    [SerializeField]
    private Text slotNameLabel;

    [SerializeField]
    private Text slotTypeLabel;

    [SerializeField]
    private Text slotStatusLabel;

    [SerializeField]
    private int teamIndex = -1;

    [SerializeField]
    private int slotIndex = -1;

    //non-serialized cache vars for the slots arrays
    private MissionDefinition.TeamDefinition teamDefinition;
    private MissionDefinition.PlayerSlot slotDefinition;
    private ActiveTeam activeTeam;
    private ActivePlayerSlot activeSlot;

    public void SetSlot(int team, int slot)
    {
        teamIndex = team;
        slotIndex = slot;

        teamDefinition = null;
        slotDefinition = null;
        activeTeam = null;
        activeSlot = null;
    }

    private void UpdateSlotRefs()
    {
        if (teamDefinition == null
            || slotDefinition == null
            || activeTeam == null
            || activeSlot == null)
        {
            var mission = MissionManager.Instance.Mission;

            teamDefinition = mission.Definition.Teams[teamIndex];
            slotDefinition = teamDefinition.Slots[slotIndex];

            activeTeam = mission.Teams[teamIndex];
            activeSlot = activeTeam.Slots[slotIndex];
        }
    }

    void Update()
    {
        if (teamIndex >= 0 && slotIndex >= 0)
        {
            UpdateSlotRefs();

            slotTypeLabel.text = slotDefinition.ShipType.name;

            slotNameLabel.text = slotDefinition.Name;
            slotStatusLabel.text = activeSlot.Status.ToString();
        }
    }

    private void ChangeStatus(bool next)
    {
        UpdateSlotRefs();

        var newStatusIndex = (int)activeSlot.Status;
        newStatusIndex += next? 1 : -1;

        var statusCount = Enum.GetValues(typeof(SlotStatus)).Length;

        if (newStatusIndex < 0)
        {
            newStatusIndex = statusCount - 1;
        }
        else if (newStatusIndex >= statusCount)
        {
            newStatusIndex = 0;
        }

        activeSlot.Status = (SlotStatus)newStatusIndex;
    }

    public void NextStatus()
    {
        ChangeStatus(true);
    }

    public void PreviousStatus()
    {
        ChangeStatus(false);
    }
}
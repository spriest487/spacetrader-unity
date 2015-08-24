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

    public void SetSlot(int team, int slot)
    {
        teamIndex = team;
        slotIndex = slot;
    }
    
    void Update()
    {
        if (teamIndex >= 0 && slotIndex >= 0)
        {
            var mission = MissionManager.Instance.Mission;

            var teamDefinition = mission.Definition.Teams[teamIndex];
            var slotDefinition = teamDefinition.Slots[slotIndex];

            var activeTeam = mission.Teams[teamIndex];
            var activeSlot = activeTeam.Slots[slotIndex];

            slotTypeLabel.text = slotDefinition.ShipType.name;

            slotNameLabel.text = slotDefinition.Name.ToUpper();
            slotStatusLabel.text = activeSlot.Status.ToString();
        }
    }

    private void ChangeStatus(bool next)
    {
        var activeSlot = MissionManager.Instance.Mission.Teams[teamIndex].Slots[slotIndex];

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

        /*singleplayer - if human was selected, change any other slots
        that are set to "human" back to "Open" */
        if (activeSlot.Status == SlotStatus.Human)
        {
            var allTeams = MissionManager.Instance.Mission.Teams;
            var teamCount = allTeams.Length;

            for (int team = 0; team < teamCount; ++team)
            {
                var allSlots = allTeams[team].Slots;
                var slotCount = allSlots.Length;

                for (int slot = 0; slot < slotCount; ++slot)
                {
                    if (team == teamIndex && slot == slotIndex)
                    {
                        continue;
                    }

                    if (allSlots[slot].Status == SlotStatus.Human)
                    {
                        allSlots[slot].Status = SlotStatus.Open;
                    }
                }
            }
        }
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
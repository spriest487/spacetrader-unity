using UnityEngine;
using UnityEngine.UI;

public class MissionTeamSlotItem : MonoBehaviour
{
    [SerializeField]
    private Text slotNameLabel;

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

            var team = mission.Definition.Teams[teamIndex];
            var slotDefinition = team.Slots[slotIndex];

            slotNameLabel.text = slotDefinition.ShipType.name;
        }
    }
}
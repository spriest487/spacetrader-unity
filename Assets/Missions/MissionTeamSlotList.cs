#pragma warning disable 0649

using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class MissionTeamSlotList : MonoBehaviour
{
    [SerializeField]
    private Transform slotList;

    [SerializeField]
    private MissionTeamSlotItem slotItemPrefab;

    [SerializeField]
    private MissionTeamDividerItem teamDividerPrefab;

    public void OnEnable()
    {
        var mission = MissionManager.Instance.Mission;

        foreach (Transform listChild in slotList.transform)
        {
            Destroy(listChild.gameObject);
        }

        var teamCount = mission.Definition.TeamCount;
        for (int teamIndex = 0; teamIndex < teamCount; ++teamIndex)
        {
            var team = mission.Definition.GetTeam(teamIndex);

            var dividerObj = Instantiate(teamDividerPrefab);
            dividerObj.transform.SetParent(slotList, false);
            dividerObj.SetTeam(teamIndex);

            var slotCount = team.SlotCount;
            for (int slotIndex = 0; slotIndex < slotCount; ++slotIndex)
            {
                var slotObj = Instantiate(slotItemPrefab);
                slotObj.transform.SetParent(slotList, false);
                slotObj.SetSlot(teamIndex, slotIndex);
            }
        }
    }
}
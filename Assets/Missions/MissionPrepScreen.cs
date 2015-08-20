using UnityEngine;
using UnityEngine.UI;

public class MissionPrepScreen : MonoBehaviour
{
    [SerializeField]
    private MissionTeamSlotItem slotItemPrefab;

    [SerializeField]
    private MissionTeamDividerItem teamDividerPrefab;

    [SerializeField]
    private Transform slotList;

    [SerializeField]
    private Text missionDescriptionText;

    [SerializeField]
    private Text missionTitleText;

    private Button readyButton;
    private Text readyText;
    
    public void Ready()
    {
        if (MissionManager.Instance.Phase == MissionPhase.Prep)
        {
            MissionManager.Instance.BeginMission();
        }

        ScreenManager.Instance.SetStates(ScreenManager.HudOverlayState.None, ScreenManager.ScreenState.Flight);
    }

    void Start()
    {
        readyButton = transform.Find("Ready Button").GetComponent<Button>();
        readyText = readyButton.GetComponentInChildren<Text>();
    }

    void OnScreenActive()
    {
        var mission = MissionManager.Instance.Mission;

        if (mission != null)
        {
            missionDescriptionText.text = mission.Definition.Description;
            missionTitleText.text = mission.Definition.MissionName.ToUpper();

            foreach (Transform listChild in slotList.transform)
            {
                Destroy(listChild.gameObject);
            }

            var teamCount = mission.Definition.Teams.Length;
            for (int teamIndex = 0; teamIndex < teamCount; ++teamIndex)
            {
                var team = mission.Definition.Teams[teamIndex];

                var dividerObj = Instantiate(teamDividerPrefab);
                dividerObj.transform.SetParent(slotList, false);
                dividerObj.SetTeam(teamIndex);

                var slotCount = team.Slots.Length;
                for (int slotIndex = 0; slotIndex < slotCount; ++slotIndex)
                {
                    var slotObj = Instantiate(slotItemPrefab);
                    slotObj.transform.SetParent(slotList, false);
                    slotObj.SetSlot(teamIndex, slotIndex);
                }
            }
        }
    }
    
    void Update()
    {
        if (MissionManager.Instance.Phase == MissionPhase.Prep)
        {
            readyText.text = "Ready";
        }
        else
        {
            readyText.text = "Close";
        }
    }
}

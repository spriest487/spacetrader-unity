using UnityEngine;
using UnityEngine.UI;

public class MissionPrepScreen : MonoBehaviour
{
    [SerializeField]
    private Transform slotItemPrefab;

    [SerializeField]
    private Transform slotList;

    [SerializeField]
    private Text missionDescriptionText;

    [SerializeField]
    private Text missionTitleText;

    public void Ready()
    {
        MissionManager.Instance.BeginMission();

        ScreenManager.Instance.SetStates(ScreenManager.HudOverlayState.None, ScreenManager.ScreenState.Flight);
    }

    void OnScreenActive()
    {
        var mission = MissionManager.Instance.Mission;

        if (mission != null)
        {
            missionDescriptionText.text = mission.Definition.Description;
            missionTitleText.text = mission.Definition.MissionName;

            for (int slot = slotList.childCount - 1; slot >= 0; --slot)
            {
                Destroy(slotList.GetChild(slot).gameObject);
            }

            foreach (var slot in mission.Players)
            {
                var slotObj = (Transform) Instantiate(slotItemPrefab);
                slotObj.SetParent(slotList, false);
            }
        }
    }
}

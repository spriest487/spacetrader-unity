#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

public class ScreensBar : MonoBehaviour
{
    [SerializeField]
    private Button missionButton;

    public void ShowMainMenu()
    {
        ScreenManager.Instance.ScreenID = ScreenID.MainMenu;
    }

    public void ShowCrew()
    {
        ScreenManager.Instance.ScreenID = ScreenID.Recruitment;
    }

    public void ShowEquipment()
    {
        ScreenManager.Instance.ScreenID = ScreenID.Equipment;
    }

    public void ShowMissionPrep()
    {
        ScreenManager.Instance.ScreenID = ScreenID.MissionPrep;
    }

    public void ShowQuests()
    {
        ScreenManager.Instance.ScreenID = ScreenID.Quests;
    }

    public void OnScreenActive()
    {
        if (missionButton)
        {
            missionButton.gameObject.SetActive(MissionManager.Instance);
        }
    }
}
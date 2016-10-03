#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

public class ScreensBar : MonoBehaviour
{
    [SerializeField]
    private Button missionButton;

    public void ShowMainMenu()
    {
        ScreenManager.Instance.FadeScreenTransition(ScreenID.MainMenu);
    }

    public void ShowCrew()
    {
        ScreenManager.Instance.FadeScreenTransition(ScreenID.Recruitment);
    }

    public void ShowEquipment()
    {
        ScreenManager.Instance.FadeScreenTransition(ScreenID.Equipment);
    }

    public void ShowMissionPrep()
    {
        ScreenManager.Instance.FadeScreenTransition(ScreenID.MissionPrep);
    }

    public void ShowQuests()
    {
        ScreenManager.Instance.FadeScreenTransition(ScreenID.Quests);
    }

    public void OnScreenActive()
    {
        if (missionButton)
        {
            missionButton.gameObject.SetActive(MissionManager.Instance);
        }
    }
}
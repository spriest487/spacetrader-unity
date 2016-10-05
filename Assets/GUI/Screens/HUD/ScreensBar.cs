#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

public class ScreensBar : MonoBehaviour
{
    [SerializeField]
    private Button missionButton;

    public void ShowMainMenu()
    {
        ScreenManager.Instance.TryFadeScreenTransition(ScreenID.MainMenu);
    }

    public void ShowCrew()
    {
        ScreenManager.Instance.TryFadeScreenTransition(ScreenID.Recruitment);
    }

    public void ShowEquipment()
    {
        ScreenManager.Instance.TryFadeScreenTransition(ScreenID.Equipment);
    }

    public void ShowMissionPrep()
    {
        ScreenManager.Instance.TryFadeScreenTransition(ScreenID.MissionPrep);
    }

    public void ShowQuests()
    {
        ScreenManager.Instance.TryFadeScreenTransition(ScreenID.Quests);
    }

    public void OnScreenActive()
    {
        if (missionButton)
        {
            missionButton.gameObject.SetActive(MissionManager.Instance);
        }
    }
}
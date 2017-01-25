#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreensListMenu : MonoBehaviour
{
    [SerializeField]
    private Transform undockButton;

    [SerializeField]
    private Transform missionButton;

    private GUIScreen guiScreen;

    private void Awake()
    {
        guiScreen = GetComponent<GUIScreen>();
    }

    public void Undock()
    {
        SpaceTraderConfig.LocalPlayer.Moorable.DockedAtStation.UndockPlayer();
    }

    public void ShowMainMenu()
    {
        GUIController.Current.SwitchTo(ScreenID.MainMenu);
    }

    public void ShowEquipment()
    {
        GUIController.Current.SwitchTo(ScreenID.Equipment);
    }

    public void ShowFleet()
    {
        GUIController.Current.SwitchTo(ScreenID.Fleet);
    }

    public void ShowMission()
    {
        GUIController.Current.SwitchTo(ScreenID.MissionPrep);
    }

    public void ShowRecruitment()
    {
        GUIController.Current.SwitchTo(ScreenID.Recruitment);
    }

    public void ShowQuests()
    {
        GUIController.Current.SwitchTo(ScreenID.Quests);
    }

    public void ShowMap()
    {
        GUIController.Current.SwitchTo(ScreenID.WorldMap);
    }

    void OnEnable()
    {
        var station = (SpaceTraderConfig.LocalPlayer && SpaceTraderConfig.LocalPlayer.Moorable)?
                SpaceTraderConfig.LocalPlayer.Moorable.DockedAtStation :
                null;

        if (station)
        {
            guiScreen.HeaderText = station.name.ToUpper();
            //headerText.text = string.Format(headerFormat, station.name.ToUpper());
        }
        else
        {
            guiScreen.HeaderText = " ";
        }

        undockButton.gameObject.SetActive(!!station);
        GetComponent<GUIScreen>().IsBackEnabled = !station;

        var mission = MissionManager.Instance? MissionManager.Instance.Mission : null;
        missionButton.gameObject.SetActive(mission != null);
    }
}

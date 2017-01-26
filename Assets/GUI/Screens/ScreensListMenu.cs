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
    private GUIController guiController;

    private void Awake()
    {
        guiScreen = GetComponent<GUIScreen>();
        guiController = GetComponentInParent<GUIController>();
    }

    public void Undock()
    {
        Universe.LocalPlayer.Dockable.DockedAtStation.UndockPlayer();
    }

    public void ShowMainMenu()
    {
        if (!guiController.HasTransition)
        {
            guiController.SwitchTo(ScreenID.MainMenu);
        }
    }

    public void ShowEquipment()
    {
        if (!guiController.HasTransition)
        {
            guiController.SwitchTo(ScreenID.Equipment);
        }
    }

    public void ShowFleet()
    {
        if (!guiController.HasTransition)
        {
            guiController.SwitchTo(ScreenID.Fleet);
        }
    }

    public void ShowMission()
    {
        if (!guiController.HasTransition)
        {
            guiController.SwitchTo(ScreenID.MissionPrep);
        }
    }

    public void ShowRecruitment()
    {
        if (!guiController.HasTransition)
        {
            guiController.SwitchTo(ScreenID.Recruitment);
        }
    }

    public void ShowQuests()
    {
        if (!guiController.HasTransition)
        {
            guiController.SwitchTo(ScreenID.Quests);
        }
    }

    public void ShowMap()
    {
        if (!guiController.HasTransition)
        {
            guiController.SwitchTo(ScreenID.WorldMap);
        }
    }

    void OnEnable()
    {
        var station = (Universe.LocalPlayer && Universe.LocalPlayer.Dockable)?
                Universe.LocalPlayer.Dockable.DockedAtStation :
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
        guiScreen.IsBackEnabled = !station;

        bool transition = guiController.HasTransition;

        var mission = MissionManager.Instance? MissionManager.Instance.Mission : null;
        missionButton.gameObject.SetActive(mission != null);
    }
}

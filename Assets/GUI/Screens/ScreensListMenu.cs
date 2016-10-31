#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreensListMenu : MonoBehaviour
{
    [SerializeField]
    private Transform undockButton;

    [SerializeField]
    private GUIScreen guiScreen;

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

    void OnEnable()
    {
        guiScreen = GetComponent<GUIScreen>();

        var player = PlayerShip.LocalPlayer;
        var station = player? player.Ship.Moorable.DockedAtStation : null;

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
    }
}

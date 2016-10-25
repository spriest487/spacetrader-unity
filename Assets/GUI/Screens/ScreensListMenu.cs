#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreensListMenu : MonoBehaviour
{
    [SerializeField]
    private string headerFormat = "{0}";

    [SerializeField]
    private Transform undockButton;

    [SerializeField]
    private Text headerText;
        
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
        var player = PlayerShip.LocalPlayer;
        var station = player? player.Ship.Moorable.DockedAtStation : null;

        if (station)
        {
            headerText.text = string.Format(headerFormat, station.name.ToUpper());
        }

        undockButton.gameObject.SetActive(!!station);
    }
}

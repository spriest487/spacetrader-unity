#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StationMenu : MonoBehaviour
{
    [SerializeField]
    private string headerFormat = "{0}";

    [SerializeField]
    private Text headerText;
            
    public void Undock()
    {
        var station = PlayerShip.LocalPlayer.Moorable.DockedAtStation;

        if (station)
        {
            //TODO?
            station.Unmoor(PlayerShip.LocalPlayer.GetComponent<Moorable>());
        }

        ScreenManager.Instance.ScreenID = ScreenID.None;
    }
    
    void OnScreenActive()
    {
        var station = PlayerShip.LocalPlayer.Moorable.DockedAtStation;

        if (station)
        {
            headerText.text = string.Format(headerFormat, station.name.ToUpper());
        }
    }
}

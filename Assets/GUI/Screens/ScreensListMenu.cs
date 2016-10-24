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
    
    private IEnumerator UndockRoutine()
    {
        yield return GUIController.Current.SwitchTo(ScreenID.HUD);

        var station = SpaceTraderConfig.LocalPlayer.Moorable.DockedAtStation;
        station.Unmoor(PlayerShip.LocalPlayer.Moorable);
    }
            
    public void Undock()
    {
        StartCoroutine(UndockRoutine());
    }
    
    void OnScreenActive()
    {
        var station = PlayerShip.LocalPlayer.Moorable.DockedAtStation;

        if (station)
        {
            headerText.text = string.Format(headerFormat, station.name.ToUpper());
        }

        undockButton.gameObject.SetActive(!!station);
    }
}

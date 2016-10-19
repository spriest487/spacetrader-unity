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
        var station = PlayerShip.LocalPlayer.Moorable.DockedAtStation;

        ScreenManager.Instance.TryFadeScreenTransition(ScreenID.None, 
            ScreenTransition.FadeToBlack,
            ScreenTransition.FadeFromBlack,
            () =>
            {
                station.Unmoor(PlayerShip.LocalPlayer.Moorable);
            });
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

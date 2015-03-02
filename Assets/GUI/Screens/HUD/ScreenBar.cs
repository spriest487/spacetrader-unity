using UnityEngine;
using System.Collections;

public class ScreenBar : MonoBehaviour
{
    public void ShowOverlay(string name)
    {
        var state = System.Enum.Parse(typeof(ScreenManager.HudOverlayState), name);

        ScreenManager.Instance.ToggleOverlay((ScreenManager.HudOverlayState) state);
    }
}

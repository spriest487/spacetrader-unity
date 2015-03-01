using UnityEngine;
using System.Collections;

public class HudOverlayCloseButton : MonoBehaviour
{
    public void CloseOverlay()
    {
        var state = ScreenManager.Instance.HudOverlay;
        ScreenManager.Instance.ToggleOverlay(state);
    }
}

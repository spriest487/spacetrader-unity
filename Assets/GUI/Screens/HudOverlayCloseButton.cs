using UnityEngine;
using System.Collections;

public class HudOverlayCloseButton : MonoBehaviour
{
    public void CloseOverlay()
    {
        var state = ScreenManager.Instance.ScreenID;
        ScreenManager.Instance.ToggleOverlay(state);
    }
}

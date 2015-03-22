using UnityEngine;

public class MissionPrepScreen : MonoBehaviour
{
    public void Ready()
    {
        MissionManager.Instance.BeginMission();

        ScreenManager.Instance.SetStates(ScreenManager.HudOverlayState.None, ScreenManager.ScreenState.Flight);
    }
}

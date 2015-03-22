using UnityEngine;
using UnityEngine.UI;

public class ScreenBar : MonoBehaviour
{
    [SerializeField]
    private Button[] missionOnlyButtons;

    [SerializeField]
    private Button[] playerButtons;

    public void ShowOverlay(string name)
    {
        var state = System.Enum.Parse(typeof(ScreenManager.HudOverlayState), name);

        ScreenManager.Instance.ToggleOverlay((ScreenManager.HudOverlayState) state);
    }

    void Update()
    {
        var missionActive = MissionManager.Instance != null;
        var playerActive = PlayerShip.LocalPlayer != null;

        foreach (var button in missionOnlyButtons)
        {
            button.gameObject.SetActive(missionActive);
        }

        foreach (var button in playerButtons)
        {
            button.interactable = playerActive;
        }
    }
}

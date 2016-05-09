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
        var state = System.Enum.Parse(typeof(ScreenID), name);

        ScreenManager.Instance.ToggleOverlay((ScreenID) state);
    }

    void Update()
    {
        var missionActive = !!MissionManager.Instance;
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

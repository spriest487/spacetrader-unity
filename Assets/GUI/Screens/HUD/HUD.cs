using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField]
    public Transform content;

    [SerializeField]
    private Transform[] playerShipInfo;

    private void Update()
    {
        bool visible = true;

        if (ScreenManager.Instance.CurrentCutscenePage != null)
        {
            visible = false;
        }

        content.gameObject.SetActive(visible);

        bool playerInfoVisible = PlayerShip.LocalPlayer ? PlayerShip.LocalPlayer.Ship : false;
        foreach (var shipInfoItem in playerShipInfo)
        {
            shipInfoItem.gameObject.SetActive(playerInfoVisible);
        }
    }
}
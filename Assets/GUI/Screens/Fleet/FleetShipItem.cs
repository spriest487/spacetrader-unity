#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

public class FleetShipItem : MonoBehaviour
{
    [Header("UI")]

    [SerializeField]
    private Button buySellButton;

    [SerializeField]
    private Text buySellText;

    [SerializeField]
    private Image selectionHighlight;

    [Header("Ship Info")]

    [SerializeField]
    private Transform shipInfoRoot;

    [SerializeField]
    private Image image;

    [SerializeField]
    private Text nameLabel;

    [Header("Empty")]

    [SerializeField]
    private Transform emptyItemRoot;

    private Ship ship;
    private SpaceStation dockedAtStation;

    public void Assign(Ship ship, SpaceStation dockedAtStation)
    {
        this.ship = ship;
        this.dockedAtStation = dockedAtStation;

        if (ship)
        {
            var shipType = ship.ShipType;
            image.sprite = shipType.Thumbnail;
            nameLabel.text = shipType.name;
        }
        else
        {
            Debug.Assert(dockedAtStation, "shouldn't create empty FleetShipItems when not docked");
        }

        shipInfoRoot.gameObject.SetActive(ship);
        emptyItemRoot.gameObject.SetActive(!ship);

        buySellText.text = ship? "SELL" : "BUY";
        if (!dockedAtStation)
        {
            buySellButton.interactable = false;
        }
    }

    public void BuyOrSell()
    {
        if (ship)
        {
            //sell
        }
        else
        {
            //buy
            SendMessageUpwards("ShowAvailableShipsPanel");
        }
    }
}
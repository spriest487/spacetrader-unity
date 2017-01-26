#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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

    [SerializeField]
    private Text emptyLabel;

    private Ship ship;
    private SpaceStation dockedAtStation;

    private FleetScreen fleetScreen;

    private void Awake()
    {
        fleetScreen = GetComponentInParent<FleetScreen>();
        Debug.Assert(fleetScreen, "fleet ship item must belong to a fleet list screen");
    }

    public void Assign(Ship ship, SpaceStation dockedAtStation)
    {
        this.ship = ship;
        this.dockedAtStation = dockedAtStation;

        if (ship)
        {
            var shipType = ship.ShipType;
            image.sprite = shipType.Thumbnail;
            nameLabel.text = shipType.name;

            //todo: "swap to" option for my ship
            bool isMyShip = Universe.LocalPlayer.Ship == ship;

            buySellButton.interactable = dockedAtStation && !isMyShip;
            buySellText.text = "SELL";

            shipInfoRoot.gameObject.SetActive(true);
            emptyItemRoot.gameObject.SetActive(false);
        }
        else
        {
            Debug.Assert(dockedAtStation, "shouldn't create empty FleetShipItems when not docked");

            shipInfoRoot.gameObject.SetActive(false);
            emptyItemRoot.gameObject.SetActive(true);

            var player = Universe.LocalPlayer;
            var localFleet = Universe.FleetManager.GetFleetOf(player.Ship);
            var fleetSize = localFleet? localFleet.Members.Count() : 0;
            var fleetCapacity = localFleet? localFleet.Capacity : 1;

            var emptyText = string.Format("Fleet Size: {0}/{1} ", fleetSize, fleetCapacity);

            var freeSlots = fleetCapacity - fleetSize;
            emptyText += (freeSlots > 0)?
                "(from Pilot skill)" :
                "(Increase Pilot skill to unlock more slots)";

            emptyLabel.text = emptyText;

            buySellButton.interactable = freeSlots > 0;
            buySellText.text = "BUY...";
        }
    }

    public void BuyOrSell()
    {
        if (ship)
        {
            //SELL

            //we're hopefully about to destroy the ship so get this first
            var soldShipName = ship.ShipType.name;
            string error;
            if (Universe.Market.TrySellShipToStation(Universe.LocalPlayer, ship, out error))
            {
                PlayerNotifications.GameMessage("Sold " +soldShipName);
                fleetScreen.Refresh();
            }
            else
            {
                PlayerNotifications.Error(error);
            }
        }
        else
        {
            //buy
            SendMessageUpwards("ShowAvailableShipsPanel");
        }
    }
}
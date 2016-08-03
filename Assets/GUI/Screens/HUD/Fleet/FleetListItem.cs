#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

public class FleetListItem : MonoBehaviour
{
    [SerializeField]
    private Text nameLabel;

    [SerializeField]
    private Image armorBar;

    [SerializeField]
    private Text wingmanStatus;

    [SerializeField]
    private Image captainPortrait;

    [SerializeField]
    private Transform selectionOverlay;

    [HideInInspector]
    [SerializeField]
    private Ship ship;

    [SerializeField]
    [HideInInspector]
    private WingmanCaptain wingmanCaptain;

    [HideInInspector]
    [SerializeField]
    private Hitpoints hitpoints;

    public static FleetListItem CreateFromPrefab(FleetListItem prefab, Ship ship, Color labelColor)
    {
        var item = Instantiate(prefab);
        item.ship = ship;
        item.hitpoints = ship.GetComponent<Hitpoints>();
        item.wingmanCaptain = ship.GetComponent<WingmanCaptain>();

        item.nameLabel.color = labelColor;
        foreach (var selectionImage in item.selectionOverlay.GetComponentsInChildren<Image>())
        {
            selectionImage.color = labelColor;
        }

        item.Update();

        return item;
    }

    private string OrderString(WingmanOrder order)
    {
        switch (order)
        {
            case WingmanOrder.Attack:
                if (ship.Target)
                {
                    return "Attacking " + ship.Target.name;
                }
                else
                {
                    return "Following";
                }
            case WingmanOrder.Follow: return "Following";
            case WingmanOrder.Wait: return "Waiting";
            default: return null;
        }
    }

    private void Update()
    {
        if (!ship)
        {
            //await sweet death, fleetlist will kill us
            return;
        }

        nameLabel.text = ship.name;

        if (hitpoints && hitpoints.GetMaxArmor() > 0)
        {
            armorBar.gameObject.SetActive(true);
            armorBar.fillAmount = hitpoints.GetArmor() / (float)hitpoints.GetMaxArmor();
        }
        else
        {
            armorBar.gameObject.SetActive(false);
        }

        var captain = ship.GetCaptain();
        if (captain)
        {
            captainPortrait.sprite = captain.Portrait;
            captainPortrait.gameObject.SetActive(true);
        }
        else
        {
            captainPortrait.gameObject.SetActive(false);
        }

        string wingmanOrder = null;
        if (wingmanCaptain)
        {
            wingmanOrder = OrderString(wingmanCaptain.ActiveOrder);
        }

        if (wingmanOrder != null)
        {
            wingmanStatus.gameObject.SetActive(true);
            wingmanStatus.text = wingmanOrder;
        }
        else
        {
            wingmanStatus.gameObject.SetActive(false);
        }

        var player = PlayerShip.LocalPlayer;
        var shipTargetable = ship.GetComponent<Targetable>();
        selectionOverlay.gameObject.SetActive(shipTargetable && player && player.Ship.Target == shipTargetable);
    }

    public void TargetMember()
    {
        var player = PlayerShip.LocalPlayer;
        var shipTargetable = ship.GetComponent<Targetable>();

        if (player && shipTargetable)
        {
            player.Ship.Target = shipTargetable;
        }
    }
}

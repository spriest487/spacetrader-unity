using UnityEngine;
using UnityEngine.UI;

public class FleetListItem : MonoBehaviour
{
    [SerializeField]
    private Text nameLabel;

    [SerializeField]
    private Image armorBar;

    [SerializeField]
    private Image captainPortrait;

    [SerializeField]
    private Transform selectionOverlay;

    [HideInInspector]
    [SerializeField]
    private Ship ship;

    [HideInInspector]
    [SerializeField]
    private Hitpoints hitpoints;

    public static FleetListItem CreateFromPrefab(FleetListItem prefab, Ship ship, Color labelColor)
    {
        var item = Instantiate(prefab);
        item.ship = ship;
        item.hitpoints = ship.GetComponent<Hitpoints>();

        item.nameLabel.color = labelColor;
        foreach (var selectionImage in item.selectionOverlay.GetComponentsInChildren<Image>())
        {
            selectionImage.color = labelColor;
        }

        item.Update();

        return item;
    }

    private void Update()
    {
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

        var captain = ship.CrewAssignments.Captain;
        if (captain)
        {
            captainPortrait.sprite = captain.Portrait;
            captainPortrait.gameObject.SetActive(true);
        }
        else
        {
            captainPortrait.gameObject.SetActive(false);
        }

        var player = PlayerShip.LocalPlayer;
        var shipTargetable = ship.GetComponent<Targetable>();
        selectionOverlay.gameObject.SetActive(player && player.Ship.Target == shipTargetable);
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

#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

public class FleetListItem : MonoBehaviour
{
    const int MaxLabelLength = 10;

    [Header("UI Elements")]

    [SerializeField]
    private Text nameLabel;

    [SerializeField]
    private Image armorBar;

    [SerializeField]
    private Text memberStatusLabel;

    [SerializeField]
    private Image captainPortrait;

    [SerializeField]
    private Transform selectionOverlay;
    
    [SerializeField, HideInInspector]
    private Ship ship;

    [SerializeField, HideInInspector]
    private CombatAI combatAI;

    [SerializeField, HideInInspector]
    private Hitpoints hitpoints;

    public void Assign(Ship ship, Color labelColor)
    {
        this.ship = ship;
        hitpoints = ship.GetComponent<Hitpoints>();
        combatAI = ship.GetComponent<CombatAI>();

        nameLabel.color = labelColor;
        foreach (var selectionImage in selectionOverlay.GetComponentsInChildren<Image>())
        {
            selectionImage.color = labelColor;
        }

        Update();
    }

    private void Update()
    {
        if (!ship)
        {
            //await sweet death, fleetlist will kill us
            return;
        }

        var label = ship.name;
        if (label.Length > MaxLabelLength)
        {
            const string ellipsis = "...";
            label = label.Substring(0, MaxLabelLength - ellipsis.Length) + ellipsis;
        }

        nameLabel.text = label;

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
        var portrait = captain ? captain.Portrait : Universe.CrewConfiguration.DefaultPortrait;

        captainPortrait.sprite = portrait;

        string combatOrderText = null;
        if (combatAI)
        {
            combatOrderText = combatAI.ActiveOrder.GetHUDLabel();
        }

        if (combatOrderText != null)
        {
            memberStatusLabel.gameObject.SetActive(true);
            memberStatusLabel.text = combatOrderText;
        }
        else
        {
            memberStatusLabel.gameObject.SetActive(false);
        }

        var player = Universe.LocalPlayer;
        var shipTargetable = ship.GetComponent<Targetable>();
        selectionOverlay.gameObject.SetActive(shipTargetable && player && player.Ship.Target == shipTargetable);
    }

    public void TargetMember()
    {
        var player = Universe.LocalPlayer;
        var shipTargetable = ship.GetComponent<Targetable>();

        if (player && shipTargetable)
        {
            player.Ship.Target = shipTargetable;
        }
    }
}

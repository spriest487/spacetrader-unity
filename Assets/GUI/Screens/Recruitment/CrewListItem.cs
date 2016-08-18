#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

public class CrewListItem : MonoBehaviour
{
    private static readonly Color AFFORDABLE_COLOR = Color.white;
    private static readonly Color UNAFFORDABLE_COLOR = Color.red;

    public enum BuySellMode
    {
        ReadOnly,
        Buyable,
        Sellable
    }
        
    [SerializeField]
    private CrewMember member;

    [Header("UI")]

    [SerializeField]
    private Button hireFireButton;

    [SerializeField]
    private Text nameLabel;

    [SerializeField]
    private Text hirePriceLabel;

    [SerializeField]
    private Text pilotSkillLabel;

    [SerializeField]
    private Text weaponsSkillLabel;

    [SerializeField]
    private Text mechSkillLabel;

    [SerializeField]
    private Image portrait;

    public CrewMember CrewMember
    {
        get { return member; }
    }

    public void Assign(CrewMember member, BuySellMode buySellMode)
    {
        this.member = member;
        nameLabel.text = member.name;
        portrait.sprite = member.Portrait;

        pilotSkillLabel.text = member.PilotSkill.ToString();
        weaponsSkillLabel.text = member.WeaponsSkill.ToString();
        mechSkillLabel.text = member.MechanicalSkill.ToString();

        if (hirePriceLabel)
        {
            var price = SpaceTraderConfig.Market.GetHirePrice(member);

            hirePriceLabel.text = "*" + price.ToString();
        }

        hireFireButton.gameObject.SetActive(buySellMode != BuySellMode.ReadOnly);
        hirePriceLabel.gameObject.SetActive(buySellMode == BuySellMode.Buyable);
    }

    void Update()
    {
        if (hirePriceLabel)
        {
            var price = SpaceTraderConfig.Market.GetHirePrice(member);
            var money = PlayerShip.LocalPlayer.Money;

            if (price > money)
            {
                hirePriceLabel.color = UNAFFORDABLE_COLOR;
            }
            else
            {
                hirePriceLabel.color = AFFORDABLE_COLOR;
            }
        }
    }

    public void HireOrFire()
    {
        var player = PlayerShip.LocalPlayer;
        var moorable = player.GetComponent<Moorable>();
        Debug.Assert(moorable, "player must have a moorable!");

        var station = moorable.SpaceStation;
        Debug.Assert(station, "must be docked!");
        
        bool hiring = station.AvailableCrew.Contains(member);

        var market = SpaceTraderConfig.Market;

        if (hiring)
        {
            var price = market.GetHirePrice(member);
            if (price <= player.Money)
            {
                SpaceTraderConfig.Market.HireCrewMember(player, station, member);
            }
        }
        else
        {
            SpaceTraderConfig.Market.FireCrewMember(player, station, member);
        }
    }
}
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
    private Image portrait;

    [SerializeField]
    private Text hireFireLabel;

    [Header("XP")]

    [SerializeField]
    private Image xpBar;

    [SerializeField]
    private Text xpLabel;

    [Header("Skills")]

    [SerializeField]
    private Text hirePriceLabel;

    [SerializeField]
    private Text pilotSkillLabel;

    [SerializeField]
    private Text weaponsSkillLabel;

    [SerializeField]
    private Text mechSkillLabel;

    private BuySellMode buySellMode;

    public CrewMember CrewMember
    {
        get { return member; }
    }

    public void Assign(CrewMember member, BuySellMode buySellMode)
    {
        this.member = member;
        nameLabel.text = member.name;
        portrait.sprite = member.Portrait;
        this.buySellMode = buySellMode;

        pilotSkillLabel.text = member.PilotSkill.ToString();
        weaponsSkillLabel.text = member.WeaponsSkill.ToString();
        mechSkillLabel.text = member.MechanicalSkill.ToString();

        var prevLevel = 0f;
        var nextLevel = 1000f; //todo

        xpBar.fillAmount = (member.XP - prevLevel) / (nextLevel - prevLevel);
        xpLabel.text = string.Format("{0}/{1} XP", member.XP, nextLevel);

        if (hirePriceLabel)
        {
            var price = SpaceTraderConfig.Market.GetHirePrice(member);

            hirePriceLabel.text = "*" + price.ToString();
        }

        hireFireButton.gameObject.SetActive(buySellMode != BuySellMode.ReadOnly);
        hireFireLabel.text = buySellMode == BuySellMode.Buyable ? "Hire" : "Dismiss";
        hirePriceLabel.gameObject.SetActive(buySellMode == BuySellMode.Buyable);
    }

    void Update()
    {
        if (buySellMode == BuySellMode.Buyable)
        {
            var price = SpaceTraderConfig.Market.GetHirePrice(member);
            var money = PlayerShip.LocalPlayer.Money;

            if (price > money)
            {
                hirePriceLabel.color = UNAFFORDABLE_COLOR;
                hireFireButton.interactable = false;
            }
            else
            {
                hirePriceLabel.color = AFFORDABLE_COLOR;
                hireFireButton.interactable = true;
            }
        }
        else
        {
            hireFireButton.interactable = true;
        }
    }

    public void HireOrFire()
    {
        var player = PlayerShip.LocalPlayer;
        var moorable = player.Moorable;
        Debug.Assert(moorable, "player must have a moorable!");

        var station = moorable.DockedAtStation;
        Debug.Assert(station, "must be docked!");

        bool hiring = member.AtStation == station;

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
using UnityEngine;
using UnityEngine.UI;

public class CrewListItem : MonoBehaviour
{
    private static readonly Color AFFORDABLE_COLOR = Color.white;
    private static readonly Color UNAFFORDABLE_COLOR = Color.red;

    [SerializeField]
    private Text nameLabel;

    [SerializeField]
    private Text hirePriceLabel;
    
    [SerializeField]
    private CrewMember member;

    public CrewMember CrewMember
    {
        get { return member; }
    }

    public static CrewListItem CreateFromPrefab(CrewListItem prefab, CrewMember member)
    {
        var instance = Instantiate(prefab);
        instance.nameLabel.text = member.name;
        instance.member = member;
        
        if (instance.hirePriceLabel)
        {
            var price = SpaceTraderConfig.Market.GetHirePrice(member);

            instance.hirePriceLabel.text = "*" +price.ToString();
        }

        return instance;
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
}
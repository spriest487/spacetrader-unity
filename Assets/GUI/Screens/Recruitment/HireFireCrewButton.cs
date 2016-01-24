using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HireFireCrewButton : MonoBehaviour
{
    private CrewListItem FindItem()
    {
        var item = GetComponentInParent<CrewListItem>();
        return item;
    }

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            var player = PlayerShip.LocalPlayer;
            var moorable = player.GetComponent<Moorable>();
            Debug.Assert(moorable != null, "can't hire/fire when not docked!");
            var station = moorable.SpaceStation;

            var crewMember = FindItem().CrewMember;

            bool hiring = station.AvailableCrew.Contains(crewMember);

            var market = SpaceTraderConfig.Market;

            if (hiring)
            {
                var price = market.GetHirePrice(crewMember);
                if (price <= player.Money)
                {
                    SpaceTraderConfig.Market.HireCrewMember(player, station, crewMember);
                }
            }
            else
            {
                SpaceTraderConfig.Market.FireCrewMember(player, station, crewMember);
            }
        });
    }
}

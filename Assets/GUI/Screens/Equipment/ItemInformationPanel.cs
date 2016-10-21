#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class ItemInformationPanel : MonoBehaviour
{
    [SerializeField]
    private ItemType itemType;

    [SerializeField]
    private bool itemOwnedByPlayer;

    [Header("Item Info Fields")]

    [SerializeField]
    private Text nameLabel;

    [SerializeField]
    private Text descriptionLabel;

    [SerializeField]
    private Image icon;

    [SerializeField]
    private MultiElementFitter statsRoot;

    [SerializeField]
    private ShipStatsEntry statsPrefab;

    private PooledList<ShipStatsEntry, KeyValuePair<string, string>> statsLines;

    private ScrollRect scrollRect;
    
    public void SetItem(ItemType type, bool ownedByPlayer)
    {
        itemType = type;
        itemOwnedByPlayer = ownedByPlayer;
        Refresh();
    }

    private void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    private void Refresh()
    {
        if (statsLines == null)
        {
            statsLines = new PooledList<ShipStatsEntry, KeyValuePair<string, string>>(statsRoot.transform, statsPrefab);
        }

        if (itemType)
        {
            nameLabel.text = itemType.DisplayName;
            icon.sprite = itemType.Icon;
            icon.gameObject.SetActive(true);
        
            descriptionLabel.text = itemType.Description;

            var stats = new List<KeyValuePair<string, string>>();
            
            var player = SpaceTraderConfig.LocalPlayer;
            var station = player.Ship.Moorable.DockedAtStation;
            var market = SpaceTraderConfig.Market;

            var baseVal = Market.FormatCurrency(itemType.BaseValue);
            stats.Add(new KeyValuePair<string, string>("Base Value", baseVal));

            if (station)
            {
                int price;
                string priceType;

                if (itemOwnedByPlayer)
                {
                    price = market.GetSellingItemPrice(itemType, station);
                    priceType= "Sells For"; 
                }
                else
                {
                    price = market.GetBuyingItemPrice(itemType, station);
                    priceType = "Price";
                }

                stats.Add(new KeyValuePair<string, string>(priceType, Market.FormatCurrency(price)));
            }

            stats.AddRange(itemType.GetDisplayedStats(itemOwnedByPlayer? player.Ship : null));

            statsLines.Refresh(stats, (i, statsLine, statsEntry) =>
                statsLine.SetText(statsEntry.Key, statsEntry.Value));
        }
        else
        {
            nameLabel.text = "No item selected";
            icon.gameObject.SetActive(false);
            descriptionLabel.text = null;

            statsLines.Clear();
        }
        
        statsRoot.Elements = statsLines.Select(l => (RectTransform) l.transform);
    }

    private void OnScreenActive()
    {
        Refresh();
    }
}

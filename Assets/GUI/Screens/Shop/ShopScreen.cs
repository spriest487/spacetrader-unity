using UnityEngine;
using UnityEngine.UI;

public class ShopScreen : MonoBehaviour
{
    [SerializeField]
    private CargoHoldList cargoItems;

    [SerializeField]
    private CargoHoldList forSaleItems;

    [SerializeField]
    private ItemInformationPanel infoPanel;

    [SerializeField]
    private Button buyButton;

    [SerializeField]
    private Button sellButton;

    private void Update()
    {
        var player = SpaceTraderConfig.LocalPlayer;
        var station = player.GetComponent<Moorable>().SpaceStation;

        cargoItems.CargoHold = player.Ship.Cargo;
        forSaleItems.CargoHold = station.ItemsForSale;

        buyButton.interactable = false;
        sellButton.interactable = false;

        if (cargoItems.HighlightedIndex >= 0)
        {
            sellButton.interactable = true;

            infoPanel.ItemType = cargoItems.HighlightedItem;
        }
        else if (forSaleItems.HighlightedIndex >= 0)
        {
            var buyingType = forSaleItems.HighlightedItem;
            infoPanel.ItemType = buyingType;

            if (forSaleItems.HighlightedIndex >= 0
                && cargoItems.CargoHold.FreeCapacity > 0)
            {
                var price = SpaceTraderConfig.Market.GetBuyingItemPrice(buyingType, station);

                if (price <= player.Money)
                {
                    buyButton.interactable = true;
                }
            }            
        }
        else
        {
            infoPanel.ItemType = null;
        }
    }
    
    private void OnSelectCargoItem(CargoHoldListItem selection) {
        /* can only select things from one list at a time */
        if (selection.CargoHold == cargoItems.CargoHold)
        {
            forSaleItems.HighlightedIndex = -1;
        }
        else
        {
            cargoItems.HighlightedIndex = -1;
        }
    }

    public void BuySelectedItem()
    {
        if (forSaleItems.HighlightedIndex < 0)
        {
            Debug.LogWarning("tried to buy item with no selection on screen");
            return;
        }

        var player = SpaceTraderConfig.LocalPlayer;
        var station = player.GetComponent<Moorable>().SpaceStation;
        SpaceTraderConfig.Market.BuyItemFromStation(player, forSaleItems.HighlightedIndex, station);
    }

    public void SellSelectedItem()
    {
        if (cargoItems.HighlightedIndex < 0)
        {
            Debug.LogWarning("tried to sell item with no selection on screen");
            return;
        }

        var player = SpaceTraderConfig.LocalPlayer;
        var station = player.GetComponent<Moorable>().SpaceStation;
        SpaceTraderConfig.Market.SellItemToStation(player, cargoItems.HighlightedIndex, station);
    }
}

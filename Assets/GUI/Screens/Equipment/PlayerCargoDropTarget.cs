#pragma warning disable 0649

using UnityEngine;
using System.Collections.Generic;

public class PlayerCargoDropTarget : MonoBehaviour
{
    [SerializeField]
    private CargoHoldListItem slot;
    
    [SerializeField]
    private CargoHoldList playerCargoList;
        
    private void OnDropCargoItem(CargoHoldListItem item)
    {
        var player = PlayerShip.LocalPlayer;
        var station = player.CurrentStation;
        var playerCargo = player.Ship.Cargo;
        var sourceCargo = item.CargoHold;

        int targetIndex;
        if (!slot)
        {
            targetIndex = System.Math.Min(item.ItemIndex, playerCargo.FirstFreeIndex);
        }
        else
        {
            targetIndex = slot.ItemIndex;
        }

        Debug.LogFormat("drop on player cargo, target slot {0}", targetIndex);

        if (targetIndex >= 0)
        {
            if (sourceCargo == playerCargo)
            {
                playerCargo.Swap(item.ItemIndex, targetIndex);
                playerCargoList.HighlightedIndex = targetIndex;
            }
            else if (station && sourceCargo == station.ItemsForSale)
            {
                SpaceTraderConfig.Market.BuyItemFromStation(player, item.ItemIndex);
                playerCargoList.HighlightedIndex = -1;
            }
        }        
    }
       
    private void OnCargoListNewItems(List<CargoHoldListItem> items)
    {
        if (!slot)
        {
            items.ForEach(item =>
            {
                var slotDropTarget = item.gameObject.AddComponent<PlayerCargoDropTarget>();
                slotDropTarget.playerCargoList = playerCargoList;
                slotDropTarget.slot = item;
            });
        }
    }
}
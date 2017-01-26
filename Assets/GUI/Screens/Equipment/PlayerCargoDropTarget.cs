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
        var station = player.Dockable.DockedAtStation;
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
                playerCargoList.HighlightedIndex = CargoHold.BadIndex;
            }
        }
    }

    private void OnDropHardpointModule(ShipModuleController module)
    {
        var player = PlayerShip.LocalPlayer;
        var targetCargo = playerCargoList.CargoHold;
        var loadout = player.Ship.ModuleLoadout;

        int targetIndex = slot ? slot.ItemIndex : targetCargo.FirstFreeIndex;
        if (targetIndex != CargoHold.BadIndex)
        {
            var targetItem = targetCargo[targetIndex];
            var targetModule = targetItem as ModuleItemType;

            if (targetItem && !targetModule)
            {
                Debug.Log("can't swap out item, swapped item is not a module");
                return;
            }

            var swappedModule = loadout.RemoveAt(module.ModuleSlot);
            if (targetItem)
            {
                loadout.Equip(module.ModuleSlot, targetModule);
            }

            targetCargo[targetIndex] = swappedModule;

            //now highlight the swapped module in the player cargo
            playerCargoList[targetIndex].OnClickCargoItem();
        }
    }
       
    private void OnCargoListNewItems(List<CargoHoldListItem> items)
    {
        //if this is applied to a single slot, ignore this
        if (slot)
        {
            return;
        }

        for (int i = 0; i < items.Count; ++i)
        {
            var item = items[i];
            var slotDropTarget = item.gameObject.GetComponent<PlayerCargoDropTarget>();
            if (!slotDropTarget)
            {
                slotDropTarget = item.gameObject.AddComponent<PlayerCargoDropTarget>();
            }

            slotDropTarget.playerCargoList = playerCargoList;
            slotDropTarget.slot = item;
        }
    }
}
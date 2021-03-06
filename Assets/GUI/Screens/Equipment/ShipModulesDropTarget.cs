﻿#pragma warning disable 0649

using UnityEngine;

public class ShipModulesDropTarget : MonoBehaviour
{
    private ShipModulesController modules;

    [SerializeField]
    private ShipModuleController module;

    private void Awake()
    {
        modules = GetComponentInParent<ShipModulesController>();
    }

    private void OnDropCargoItem(CargoHoldListItem droppedItem)
    {
        var player = Universe.LocalPlayer;
        var loadout = player.Ship.ModuleLoadout;
        var droppedModuleType = droppedItem.ItemType as ModuleItemType;

        if (droppedModuleType)
        {
            int targetSlot = module ? module.ModuleSlot : loadout.FindFirstFreeSlot();

            if (loadout.IsValidSlot(targetSlot))
            {
                if (!loadout.IsFreeSlot(targetSlot))
                {
                    //swap to cargo
                    var currentItem = loadout.GetSlot(targetSlot).ModuleType;

                    droppedItem.CargoHold[droppedItem.ItemIndex] = currentItem;
                    loadout.RemoveAt(targetSlot);
                }
                else
                {
                    droppedItem.CargoHold[droppedItem.ItemIndex] = null;
                }

                loadout.Equip(targetSlot, droppedModuleType);

                modules[targetSlot].OnClickModule();
            }
            else
            {
                PlayerNotifications.Error("No free slots");
            }
        }
        else
        {
            PlayerNotifications.Error("Can only equip Modules");
        }
    }

    private void OnDropHardpointModule(ShipModuleController droppedModule)
    {
        if (!module)
        {
            //only drop areas associated with a slot can be swapped like this
            return;
        }

        var player = Universe.LocalPlayer;
        var loadout = player.Ship.ModuleLoadout;

        loadout.Swap(module.ModuleSlot, droppedModule.ModuleSlot);
    }
}
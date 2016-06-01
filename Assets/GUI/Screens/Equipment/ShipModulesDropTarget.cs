#pragma warning disable 0649

using UnityEngine;

public class ShipModulesDropTarget : MonoBehaviour
{
    private EquipmentScreen equipmentScreen;

    [SerializeField]
    private ShipModuleController module;

    private void Start()
    {
        equipmentScreen = GetComponentInParent<EquipmentScreen>();
    }
    
    [SerializeField]
    private void OnDropCargoItem(CargoHoldListItem droppedItem)
    {
        var player = PlayerShip.LocalPlayer;
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
                    var currentItem = loadout.HardpointModules[targetSlot].ModuleType;
                    
                    droppedItem.CargoHold[droppedItem.ItemIndex] = currentItem;
                    loadout.RemoveAt(targetSlot);
                }

                loadout.Equip(targetSlot, droppedModuleType);
            }
        }
        else
        {
            equipmentScreen.ShowError("Can only equip Modules");
        }
    }
}
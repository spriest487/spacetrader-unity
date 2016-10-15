#pragma warning disable 0649

using UnityEngine;
using System.Collections.Generic;

public class TargetCargoDropTarget : MonoBehaviour
{
    [SerializeField]
    private CargoHoldListItem slot;

    [SerializeField]
    private EquipmentScreen equipmentScreen;
    
    private void OnDropCargoItem(CargoHoldListItem item)
    {
        var player = PlayerShip.LocalPlayer;

        var sourceCargo = item.CargoHold;

        if (sourceCargo == equipmentScreen.PlayerCargo)
        {
            SpaceTraderConfig.Market.SellItemToStation(player, item.ItemIndex);

            //this slot should now be empty, doing this will "select" the empty slot
            item.OnClickCargoItem();
        }
    }

    private void OnCargoListNewItems(List<CargoHoldListItem> items)
    {
        if (slot)
        {
            return;
        }

        for (int i = 0; i < items.Count; ++i)
        {
            var slotDropTarget = gameObject.GetComponent<TargetCargoDropTarget>();
            if (!slotDropTarget)
            {
                slotDropTarget = gameObject.AddComponent<TargetCargoDropTarget>();
            }

            slotDropTarget.equipmentScreen = equipmentScreen;
            slotDropTarget.slot = items[i];
        }
    }
}

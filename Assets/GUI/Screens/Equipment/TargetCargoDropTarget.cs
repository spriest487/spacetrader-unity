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
        var station = player.CurrentStation;

        var sourceCargo = item.CargoHold;
        var targetCargo = equipmentScreen.TargetCargo;

        if (sourceCargo == equipmentScreen.PlayerCargo)
        {
            SpaceTraderConfig.Market.SellItemToStation(player, item.ItemIndex);
        }
    }

    private void OnCargoListNewItems(List<CargoHoldListItem> items)
    {
        if (!slot)
        {
            items.ForEach(item =>
            {
                var slotDropTarget = gameObject.AddComponent<TargetCargoDropTarget>();
                slotDropTarget.equipmentScreen = equipmentScreen;
                slotDropTarget.slot = item;
            });
        }
    }
}

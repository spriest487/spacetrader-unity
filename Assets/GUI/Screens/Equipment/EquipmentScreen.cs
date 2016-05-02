using UnityEngine;
using UnityEngine.Events;

public class EquipmentScreen : MonoBehaviour
{
    [SerializeField]
    private ItemInformationPanel infoPanel;

    [SerializeField]
    private CargoHoldList playerCargoList;

    private void OnSelectShipModule(ShipModuleController moduleController)
    {
        infoPanel.ItemType = moduleController.Module.ModuleType;
    }

    private void OnSelectCargoItem(CargoHoldListItem selection)
    {
        infoPanel.ItemType = selection.Item;
    }

    private void Update()
    {
        var player = SpaceTraderConfig.LocalPlayer;
        playerCargoList.CargoHold = player? player.Ship.Cargo : null;
    }
}
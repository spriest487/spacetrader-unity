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
        var moduleDef = moduleController.Module.ModuleType;
        var itemType = SpaceTraderConfig.CargoItemConfiguration.FindType(moduleDef.name);
        infoPanel.ItemType = itemType;
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
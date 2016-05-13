#pragma warning disable 0649

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EquipmentScreen : MonoBehaviour
{
    [SerializeField]
    private ItemInformationPanel infoPanel;

    [SerializeField]
    private CargoHoldList playerCargoList;

    [SerializeField]
    private ShipModulesController shipModules;

    [SerializeField]
    private Image dragItem; 

    private void OnSelectShipModule(ShipModuleController moduleController)
    {
        infoPanel.ItemType = moduleController.Module.ModuleType;
        playerCargoList.HighlightedIndex = -1;
    }

    private void OnSelectCargoItem(CargoHoldListItem selection)
    {
        infoPanel.ItemType = selection.Item;
        shipModules.HighlightedIndex = -1;
    }

    private void OnDragCargoItem(CargoHoldListItem selection)
    {
        dragItem.sprite = selection.Item.Icon;
    }

    private void Update()
    {
        var player = SpaceTraderConfig.LocalPlayer;
        playerCargoList.CargoHold = player? player.Ship.Cargo : null;
    }

    public void Close()
    {
        ScreenManager.Instance.ScreenID = ScreenID.None;
    }
}
#pragma warning disable 0649

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class EquipmentScreen : MonoBehaviour, IDropHandler
{
    [SerializeField]
    private ItemInformationPanel infoPanel;

    [SerializeField]
    private CargoHoldList playerCargoList;

    [SerializeField]
    private ShipModulesController shipModules;

    [SerializeField]
    private DragItem dragItem;
    
    private void OnScreenActive()
    {
        dragItem.gameObject.SetActive(false);
        var player = SpaceTraderConfig.LocalPlayer;
        playerCargoList.CargoHold = player ? player.Ship.Cargo : null;

        playerCargoList.Refresh();
        shipModules.Refresh();
    }

    private void OnSelectShipModule(ShipModuleController moduleController)
    {
        infoPanel.ItemType = moduleController.Module.ModuleType;
        playerCargoList.HighlightedIndex = -1;
    }

    private void OnSelectCargoItem(CargoHoldListItem selection)
    {
        infoPanel.ItemType = selection.ItemType;
        shipModules.HighlightedIndex = -1;
    }

    private void OnDragCargoItem(CargoHoldListItem dragged)
    {
        dragItem.gameObject.SetActive(true);
        dragItem.Item = dragged;
    }

    private void OnDropCargoItem(CargoHoldListItem dragged)
    {
        dragItem.gameObject.SetActive(false);
        dragItem.Item = null;

        var pointerEvent = new PointerEventData(EventSystem.current);
    }

    public void OnDrop(PointerEventData pointerData)
    {
        dragItem.gameObject.SetActive(false);
    }
    
    public void Close()
    {
        ScreenManager.Instance.ScreenID = ScreenID.None;
    }
}
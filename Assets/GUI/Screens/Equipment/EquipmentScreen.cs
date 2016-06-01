#pragma warning disable 0649

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class EquipmentScreen : MonoBehaviour, IDropHandler
{
    [SerializeField]
    private ItemInformationPanel infoPanel;

    [SerializeField]
    private CargoHoldList playerCargoList;

    [SerializeField]
    private CargoHoldList targetCargoList;

    [SerializeField]
    private ShipModulesController shipModules;

    [SerializeField]
    private DragItem dragItem;

    [SerializeField]
    private ErrorMessage errorMessage;

    public CargoHold TargetCargo
    {
        get
        {
            return targetCargoList.CargoHold;
        }
    }

    public CargoHold PlayerCargo
    {
        get
        {
            return playerCargoList.CargoHold;
        }
    }
    
    private void OnScreenActive()
    {
        dragItem.gameObject.SetActive(false);
        var player = SpaceTraderConfig.LocalPlayer;
        playerCargoList.CargoHold = player ? player.Ship.Cargo : null;

        playerCargoList.Refresh();
        shipModules.Refresh();
        errorMessage.Reset();

        if (player.CurrentStation)
        {
            targetCargoList.gameObject.SetActive(true);
            targetCargoList.Refresh();
            targetCargoList.CargoHold = player.CurrentStation.ItemsForSale;
        }
        else
        {
            targetCargoList.gameObject.SetActive(false);
        }
    }

    private void OnSelectShipModule(ShipModuleController moduleController)
    {
        infoPanel.ItemType = moduleController.Module.ModuleType;
        playerCargoList.HighlightedIndex = -1;
        targetCargoList.HighlightedIndex = -1;
    }

    private void OnSelectCargoItem(CargoHoldListItem selection)
    {
        infoPanel.ItemType = selection.ItemType;
        shipModules.HighlightedIndex = -1;

        if (selection.CargoHold == playerCargoList.CargoHold)
        {
            targetCargoList.HighlightedIndex = -1;
        }
        else
        {
            playerCargoList.HighlightedIndex = -1;
        }
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

    public void ShowError(string message)
    {
        errorMessage.ShowError(message);
    }
    
    public void Close()
    {
        ScreenManager.Instance.ScreenID = ScreenID.None;
    }
}
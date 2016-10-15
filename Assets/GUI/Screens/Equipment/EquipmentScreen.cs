﻿#pragma warning disable 0649

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
    private Transform targetCargoPanel;
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
        targetCargoList = targetCargoPanel.GetComponentInChildren<CargoHoldList>();
        Debug.Assert(!!targetCargoList);

        dragItem.gameObject.SetActive(false);
        var player = SpaceTraderConfig.LocalPlayer;
        playerCargoList.CargoHold = player ? player.Ship.Cargo : null;

        playerCargoList.Refresh();
        shipModules.Refresh();
        errorMessage.Reset();

        if (player.Moorable.DockedAtStation)
        {
            targetCargoPanel.gameObject.SetActive(true);
            targetCargoList.Refresh();
            targetCargoList.CargoHold = player.Moorable.DockedAtStation.ItemsForSale;
        }
        else
        {
            targetCargoPanel.gameObject.SetActive(false);
        }

        targetCargoList.HighlightedIndex = CargoHold.BadIndex;
        playerCargoList.HighlightedIndex = CargoHold.BadIndex;
        shipModules.HighlightedIndex = ModuleLoadout.BadIndex;
    }

    private void Update()
    {
        var infoType = playerCargoList.HighlightedItem;

        if (!infoType && targetCargoList.isActiveAndEnabled)
        {
            infoType = targetCargoList.HighlightedItem;
        }

        if (!infoType)
        {
            var module = shipModules.HighlightedModule;
            if (module)
            {
                infoType = module.Module.ModuleType;
            }
        }

        infoPanel.ItemType = infoType;
    }

    public void OnSelectShipModule(ShipModuleController moduleController)
    {
        playerCargoList.HighlightedIndex = -1;
        targetCargoList.HighlightedIndex = -1;
    }

    public void OnSelectCargoItem(CargoHoldListItem selection)
    {
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
    }

    private void OnDragHardpointModule(ShipModuleController module)
    {
        dragItem.gameObject.SetActive(true);
        dragItem.Module = module;
    }

    private void OnDropHardpointModule(ShipModuleController module)
    {
        dragItem.gameObject.SetActive(false);
        dragItem.Module = null;
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
        ScreenManager.Instance.TryFadeScreenTransition(ScreenID.None);
    }
}
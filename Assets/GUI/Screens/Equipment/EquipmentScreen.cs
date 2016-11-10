#pragma warning disable 0649

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(GUIScreen))]
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
        
    private void OnEnable()
    {
        targetCargoList = targetCargoPanel.GetComponentInChildren<CargoHoldList>();
        Debug.Assert(!!targetCargoList);

        dragItem.gameObject.SetActive(false);
        var player = SpaceTraderConfig.LocalPlayer;
        playerCargoList.CargoHold = player ? player.Ship.Cargo : null;

        playerCargoList.Refresh();
        shipModules.Refresh();

        if (player && player.Moorable.DockedAtStation)
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
        bool infoOwnedByPlayer = true;

        if (!infoType && targetCargoList.isActiveAndEnabled)
        {
            infoType = targetCargoList.HighlightedItem;
            infoOwnedByPlayer = false;
        }

        if (!infoType)
        {
            var module = shipModules.HighlightedModule;
            if (module)
            {
                infoType = module.Module.ModuleType;
                infoOwnedByPlayer = true;
            }
        }

        infoPanel.SetItem(infoType, infoOwnedByPlayer);
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
}
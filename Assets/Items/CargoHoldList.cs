﻿#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CargoHoldList : MonoBehaviour
{
    [SerializeField]
    private CargoHold cargoHold;

    [SerializeField]
    private int highlightedIndex;

    [SerializeField]
    private CargoHoldListItem listItem;

    [SerializeField]
    private Transform itemsHolder;

    [SerializeField]
    private string sizeFormat = "{0}/{1}";

    [SerializeField]
    private Text sizeLabel;
    
    private PooledList<CargoHoldListItem, ItemType> currentItems;

    public CargoHold CargoHold
    {
        get { return cargoHold; }
        set { cargoHold = value; }
    }

    public int HighlightedIndex
    {
        get
        {
            if (cargoHold.IsValidIndex(highlightedIndex))
            {
                return -1;
            }

            return highlightedIndex;
        }
        set
        {
            highlightedIndex = value;
            UpdateHighlight();
        }
    }

    public ItemType HighlightedItem
    {
        get
        {
            var index = HighlightedIndex;
            if (index < 0)
            {
                return null;
            }

            return cargoHold[index];
        }
    }

    private void UpdateHighlight()
    {
        if (currentItems != null)
        {
            foreach (var item in currentItems)
            {
                item.Highlighted = highlightedIndex == item.ItemIndex;
            }
        }
    }
    
    private void Update()
    {
        Refresh();
        UpdateHighlight();
    }

    public void Refresh()
    {
        if (currentItems == null)
        {
            currentItems = new PooledList<CargoHoldListItem, ItemType>(itemsHolder);
        }

        var newItems = new List<CargoHoldListItem>();

        if (cargoHold)
        {
            currentItems.Refresh(CargoHold.Items,
                (i, cargoItem) =>
                {
                    var newListItem = CargoHoldListItem.CreateFromPrefab(listItem, CargoHold, i);
                    newItems.Add(newListItem);
                    return newListItem;
                },
                (i, existingItem, cargoItem) =>
                {
                    existingItem.Assign(CargoHold, i);
                });

            highlightedIndex = System.Math.Min(highlightedIndex, CargoHold.ItemCount - 1);
        }
        else
        {
            currentItems.Clear();

            highlightedIndex = CargoHold.BAD_INDEX;
        }
    }

    private void OnSelectCargoItem(CargoHoldListItem item)
    {
        highlightedIndex = item.ItemIndex;
    }

    private void OnDragCargoItem(CargoHoldListItem item)
    {
        highlightedIndex = item.ItemIndex;

    }
}

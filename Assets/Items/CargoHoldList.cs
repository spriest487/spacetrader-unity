﻿#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

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
    
    private List<ItemType> currentItems;

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
        foreach (Transform child in itemsHolder.transform)
        {
            var item = child.GetComponent<CargoHoldListItem>();
            item.Highlighted = highlightedIndex == item.ItemIndex;
        }
    }

    private void Prepare(int capacity)
    {
        int item = 0;
        foreach (CargoHoldListItem child in itemsHolder.transform)
        {
            if (item >= capacity)
            {
                child.gameObject.SetActive(false);
            }
            ++item;
        }

        while (item < capcity)
        {

        }

        currentItems = null;
        highlightedIndex = -1;
    }

    private void Update()
    {
        Refresh();
        UpdateHighlight();
    }

    public void Refresh()
    {
        if (!CargoHold)
        {
            Prepare(0);
        }
        else
        {
            if (sizeLabel)
            {
                sizeLabel.text = string.Format(sizeFormat, CargoHold.ItemCount, CargoHold.Size);
            }

            if (currentItems == null || !currentItems.ElementsEquals(CargoHold.Items))
            {
                Prepare(CargoHold.Size);

                int oldHighlight = highlightedIndex;

                var itemCount = CargoHold.Size;
                for (int itemIndex = 0; itemIndex < itemCount; ++itemIndex)
                {
                    var item = CargoHoldListItem.CreateFromPrefab(listItem, CargoHold, itemIndex);
                    item.transform.SetParent(itemsHolder.transform, false);

                    SendMessageUpwards("OnCargoListNewItem", item, SendMessageOptions.DontRequireReceiver);
                }

                currentItems = new List<ItemType>(CargoHold.Items);

                highlightedIndex = System.Math.Min(oldHighlight, currentItems.Count - 1);
            }
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

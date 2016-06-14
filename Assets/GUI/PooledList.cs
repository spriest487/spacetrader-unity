using System;
using System.Collections.Generic;
using UnityEngine;

public class PooledList<TItem, TData>
    where TItem: MonoBehaviour
{
    public delegate TItem CreateItemCallback(TData source);
    public delegate void UpdateItemCallback(TItem existing, TData source);

    private Transform root;

    private List<TItem> currentItems;
    private List<TData> currentData;

    public PooledList(Transform root)
    {
        currentItems = new List<TItem>(root.GetComponentsInChildren<TItem>());
        currentData = null;
        this.root = root;
    }

    public void Clear()
    {
        currentItems.ForEach(item => item.gameObject.SetActive(false));
        currentData = null;
    }
    
    public void Refresh(IEnumerable<TData> data, CreateItemCallback onNewItem, UpdateItemCallback onUpdateItem)
    {
        if (currentData != null && currentData.ElementsEquals(data))
        {
            //already up to date
            return;
        }

        currentData = new List<TData>(data);
        
        int existingItemsCount = currentItems.Count;
        int newCount = currentData.Count;

        if (existingItemsCount < newCount)
        {
            currentItems.Resize(newCount);
        }

        int dataItem;
        for (dataItem = 0; dataItem < newCount; ++dataItem)
        {
            var dataValue = currentData[dataItem];
            if (dataItem < existingItemsCount)
            {
                onUpdateItem(currentItems[dataItem], dataValue);
            }
            else
            {
                var newItem = currentItems[dataItem] = onNewItem(dataValue);
                
                newItem.transform.SetParent(root, false);
            }
        }

        //if there are less items than previously, deactivate any extras
        while (dataItem < existingItemsCount)
        {
            currentItems[dataItem].gameObject.SetActive(false);
            ++dataItem;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PooledList<TItem, TData> : IEnumerable<TItem>
    where TItem: MonoBehaviour
{
    public delegate TItem CreateItemCallback(int index, TData source);
    public delegate void UpdateItemCallback(int index, TItem existing, TData source);

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
    
    public void Refresh(IEnumerable<TData> data, CreateItemCallback onNewItem, UpdateItemCallback onUpdateItem = null)
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

        int itemIndex;
        for (itemIndex = 0; itemIndex < newCount; ++itemIndex)
        {
            var dataValue = currentData[itemIndex];
            if (itemIndex < existingItemsCount)
            {
                if (onUpdateItem != null)
                {
                    onUpdateItem(itemIndex, currentItems[itemIndex], dataValue);
                }
            }
            else
            {
                var newItem = currentItems[itemIndex] = onNewItem(itemIndex, dataValue);
                
                newItem.transform.SetParent(root, false);
            }
        }

        //if there are less items than previously, deactivate any extras
        while (itemIndex < existingItemsCount)
        {
            currentItems[itemIndex].gameObject.SetActive(false);
            ++itemIndex;
        }
    }

    public IEnumerator<TItem> GetEnumerator()
    {
        if (currentItems == null)
        {
            yield break;
        }

        foreach (var item in currentItems)
        {
            yield return item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
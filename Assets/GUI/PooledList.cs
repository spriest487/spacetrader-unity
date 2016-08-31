using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class PooledList<TItem, TData> : IEnumerable<TItem>
    where TItem: MonoBehaviour
{
    public delegate void UpdateItemCallback(int index, TItem existing, TData source);

    private TItem itemPrefab;

    private Transform root;

    private List<TItem> currentItems;
    private List<TData> currentData;

    public int Count
    {
        get { return currentItems == null? 0 : currentItems.Count; }
    }

    public TItem this[int index]
    {
        get { return currentItems == null ? null : currentItems[index]; }
    }

    public IEnumerable<TData> Data
    {
        get { return currentData != null ? currentData : Enumerable.Empty<TData>(); }
    }

    public PooledList(Transform root, TItem itemPrefab)
    {
        currentItems = new List<TItem>(root.GetComponentsInChildren<TItem>());
        currentData = null;
        this.root = root;
        this.itemPrefab = itemPrefab;
    }

    public void Clear()
    {
        currentItems.ForEach(item => item.gameObject.SetActive(false));

        if (currentData != null)
        {
            currentData.Clear();
        }
    }
    
    public void Refresh(IEnumerable<TData> data, UpdateItemCallback onUpdateItem)
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

            TItem item;
            if (itemIndex >= existingItemsCount)
            {
                item = currentItems[itemIndex] = UnityEngine.Object.Instantiate(itemPrefab);
                item.transform.SetParent(root, false);
            }
            else
            {
                item = currentItems[itemIndex];
            }

            item.gameObject.SetActive(true);

            if (onUpdateItem != null)
            {
                onUpdateItem(itemIndex, item, dataValue);
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
        if (currentItems != null)
        {
            return currentItems.GetEnumerator();
        }
        else
        {
            return Enumerable.Empty<TItem>().GetEnumerator();
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
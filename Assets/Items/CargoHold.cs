using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CargoHold : ScriptableObject
{
    [SerializeField]
    private List<ItemType> items = new List<ItemType>();

    public IEnumerable<ItemType> Items
    {
        get { return items; }
    }
    
    public int Size
    {
        get
        {
            return items.Count;
        }
        set
        {
            items.Resize(value);
        }
    }

    public int FreeCapacity
    {
        get
        {
            int empty = 0;
            items.ForEach(item => 
            {
                if (item == null)
                {
                    ++empty;
                }
            });
            return empty;
        }
    }

    public int ItemCount
    {
        get
        {
            return items.Count - FreeCapacity;
        }
    }

    public int FirstFreeIndex
    {
        get
        {
            for (int i = 0; i < items.Count; ++i)
            {
                if (items[i] == null)
                {
                    return i;
                }
            }

            return -1;
        }
    }

    /// <summary>
    /// Adds an item in the first empty slot
    /// </summary>
    /// <param name="item">non-null item type to add</param>
    public void Add(ItemType item)
    {
        if (!item)
        {
            throw new System.ArgumentException("item must not be null");
        }

        Debug.AssertFormat(FreeCapacity > 0, "cargo hold does not have space to add item {0}, size is {1} and free capacity is {2}",
            item, Size, FreeCapacity);

        for (int index = 0; index < items.Count; ++index)
        {
            if (items[index] == null)
            {
                items[index] = item;
                break;
            }
        }
    }

    public void RemoveAt(int index)
    {
        if (IsValidIndex(index))
        {
            items[index] = null;
        }
    }

    public void Swap(int src, int dest)
    {
        if (!IsValidIndex(src) || !IsValidIndex(dest))
        {
            throw new System.ArgumentException("invalid indices for swap");
        }

        var old = items[dest];
        items[dest] = items[src];
        items[src] = old;
    }

    public bool IsValidIndex(int index)
    {
        return index >= 0 && index < items.Count;
    }

    public bool IsIndexFree(int index)
    {
        return IsValidIndex(index) && items[index] == null;
    }

    public ItemType this[int index]
    {
        get
        {
            return items[index];
        }
        set
        {
            items[index] = value;
        }
    }
}

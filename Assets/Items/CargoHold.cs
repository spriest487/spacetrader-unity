using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CargoHold : ScriptableObject
{
    public const int BAD_INDEX = -1;

    [SerializeField]
    private List<ItemType> items = new List<ItemType>();

    public IEnumerable<ItemType> Items
    {
        get { return items; }
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
            return Size - FreeCapacity;
        }
    }

    public int FirstFreeIndex
    {
        get
        {
            for (int i = 0; i < Size; ++i)
            {
                if (this[i] == null)
                {
                    return i;
                }
            }

            return BAD_INDEX;
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
            if (this[index] == null)
            {
                this[index] = item;
                break;
            }
        }
    }

    public void RemoveAt(int index)
    {
        if (IsValidIndex(index))
        {
            this[index] = null;
        }
    }

    public void Swap(int src, int dest)
    {
        if (!IsValidIndex(src) || !IsValidIndex(dest))
        {
            throw new System.ArgumentException("invalid indices for swap");
        }

        var old = this[dest];
        this[dest] = this[src];
        this[src] = old;
    }

    public bool IsValidIndex(int index)
    {
        return index >= 0 && index < Size;
    }

    public bool IsIndexFree(int index)
    {
        return IsValidIndex(index) && this[index] == null;
    }
}

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CargoHold : ScriptableObject
{
    public const int BadIndex = -1;

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

            return BadIndex;
        }
    }

    /// <summary>
    /// Adds an item in the first empty slot
    /// </summary>
    /// <param name="item">non-null item type to add</param>
    public void Add(ItemType item)
    {
        Debug.Assert(!!item, "item must not be null");
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

    public ItemType RemoveAt(int index)
    {
        Debug.Assert(IsValidIndex(index), "RemoveAt should only reference valid indices");

        var removed = this[index];

        this[index] = null;
        return removed;
    }

    public void Swap(int src, int dest)
    {
        Debug.Assert(IsValidIndex(src) && IsValidIndex(dest), "Swap should only reference valid indices");

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

    public bool ContainsItems(ItemType itemType, int quantity)
    {
        int count = 0;

        for (int slot = 0; slot < Size; ++slot)
        {
            if (this[slot] == itemType)
            {
                ++count;
            }
        }

        return count >= quantity;
    }
}

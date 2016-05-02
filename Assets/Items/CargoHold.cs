using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CargoHold : ScriptableObject
{
    [SerializeField]
    private List<ItemType> items = new List<ItemType>();

    [SerializeField]
    private int size = 0;

    public IEnumerable<ItemType> Items
    {
        get { return items; }
    }
    
    public int Size
    {
        get
        {
            return size;
        }
        set
        {
            if (value < size && items != null)
            {
                items.Resize(value);
            }
            size = value;
        }
    }

    public int FreeCapacity
    {
        get
        {
            return size - items.Count;
        }
    }

    public int ItemCount
    {
        get
        {
            return items.Count;
        }
    }

    public void Add(ItemType item)
    {
        Debug.AssertFormat(FreeCapacity > 0, "cargo hold does not have space to add item {0}, size is {1} and free capacity is {2}",
            item, size, FreeCapacity);
        items.Add(item);
    }

    public void RemoveAt(int index)
    {
        if (index < items.Count)
        {
            items.RemoveAt(index);
        }
    }

    public bool IsValidIndex(int index)
    {
        return index >= 0 && index < items.Count;
    }

    public ItemType this[int index]
    {
        get
        {
            return items[index];
        }
    }
}

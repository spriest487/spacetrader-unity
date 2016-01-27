using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CargoHold : ScriptableObject
{
    [SerializeField]
    private List<string> items;

    [SerializeField]
    private int size;

    public IList<string> Items
    {
        get
        {
            if (items == null)
            {
                items = new List<string>(size);
            }
            return items;
        }
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
            return size - Items.Count;
        }
    }

    public void Add(string item)
    {
        Debug.AssertFormat(FreeCapacity > 0, "cargo hold does not have space to add item {0}, size is {1} and free capacity is {2}",
            item, size, FreeCapacity);
        Items.Add(item);
    }

    public void RemoveAt(int index)
    {
        if (index < items.Count)
        {
            Items.RemoveAt(index);
        }
    }
}

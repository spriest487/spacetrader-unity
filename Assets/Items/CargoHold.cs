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
        Debug.Assert(FreeCapacity > 0);
        Items.Add(item);
    }

    public void RemoveAt(int index)
    {
        Items.RemoveAt(index);
    }
}

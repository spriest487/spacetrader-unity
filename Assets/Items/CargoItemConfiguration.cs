using UnityEngine;
using System.Collections.Generic;

public class CargoItemConfiguration : ScriptableObject
{
    [SerializeField]
    private List<ItemType> itemTypes;

    public IEnumerable<ItemType> ItemTypes
    {
        get { return itemTypes; }
    }

    private Dictionary<string, ItemType> typesByName;
    private Dictionary<string, ItemType> TypesByName
    {
        get
        {
            typesByName = new Dictionary<string, ItemType>();

            foreach (ItemType type in itemTypes)
            {
                if (type != null)
                {
                    typesByName.Add(type.name, type);
                }
            }

            return typesByName;
        }
    }

    public ItemType FindType(string name)
    {
        Debug.Assert(TypesByName.ContainsKey(name), "missing item type: " + name);

        return typesByName[name];
    }
}
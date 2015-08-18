using UnityEngine;
using System.Collections.Generic;

public class CargoItemConfiguration : MonoBehaviour
{
    [SerializeField]
    private CargoItemType[] itemTypes;

    private Dictionary<string, CargoItemType> typesByName;

    void Start()
    {
        typesByName = new Dictionary<string, CargoItemType>();

        foreach (CargoItemType type in itemTypes)
        {
            if (type != null)
            {
                typesByName.Add(type.name, type);
            }
        }
    }

    public CargoItemType FindType(string name)
    {
        CargoItemType result;
        if (typesByName.TryGetValue(name, out result))
        {
            return result;
        }
        else
        {
            return null;
        }
    }
}
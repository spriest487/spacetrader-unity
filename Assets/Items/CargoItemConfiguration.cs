using UnityEngine;
using System.Collections.Generic;

public class CargoItemConfiguration : ScriptableObject
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Create/SpaceTrader/Items/Item configuration")]
    public static void CreateNewConfiguration()
    {
        ScriptableObjectUtility.CreateAsset<CargoItemConfiguration>();
    }
#endif

    [SerializeField]
    private List<CargoItemType> itemTypes;

    public IList<CargoItemType> ItemTypes
    {
        get { return itemTypes; }
    }

    private Dictionary<string, CargoItemType> typesByName;
    private Dictionary<string, CargoItemType> TypesByName
    {
        get
        {
            typesByName = new Dictionary<string, CargoItemType>();

            foreach (CargoItemType type in itemTypes)
            {
                if (type != null)
                {
                    typesByName.Add(type.name, type);
                }
            }

            return typesByName;
        }
    }

    public CargoItemType FindType(string name)
    {
        CargoItemType result;
        if (TypesByName.TryGetValue(name, out result))
        {
            return result;
        }
        else
        {
            return null;
        }
    }
}
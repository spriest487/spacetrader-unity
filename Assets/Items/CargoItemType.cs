using UnityEngine;
using System.Collections;

public class CargoItemType : ScriptableObject
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Create/SpaceTrader/Items/Simple item type")]
    public static void Create()
    {
        ScriptableObjectUtility.CreateAsset<CargoItemType>();
    }
#endif

    [SerializeField]
    string displayName;

    [SerializeField]
    int baseValue;

    [SerializeField]
    ModuleDefinition module;
    
    public string DisplayName
    {
        get
        {
            if (module != null)
            {
                return module.name;
            }

            if (displayName != null)
            {
                return displayName;
            }

            return name;
        }
    }

    public int BaseValue
    {
        get { return baseValue; }
    }

    public ModuleDefinition ModuleDefinition
    {
        get { return module; }
    }
}

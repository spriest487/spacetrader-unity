using UnityEngine;
using System.Collections;

public class CargoItemType : ScriptableObject
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Create/SpaceTrader/Simple Cargo Item Type")]
    public static void Create()
    {
        ScriptableObjectUtility.CreateAsset<CargoItemType>();
    }
#endif

    [SerializeField]
    string displayName;

    [SerializeField]
    int baseValue;
    
    public string DisplayName { get { return displayName != null? displayName : name; } }    
    public int BaseValue { get { return baseValue; } }
}

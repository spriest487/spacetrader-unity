using UnityEngine;

public class CargoItemType : ItemType
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Create/SpaceTrader/Items/Cargo item type")]
    public static void Create()
    {
        ScriptableObjectUtility.CreateAsset<CargoItemType>();
    }
#endif

    [SerializeField]
    private string displayName;

    [TextArea]
    [SerializeField]
    private string description;
    
    public override string DisplayName
    {
        get
        {
            if (displayName != null)
            {
                return displayName;
            }

            return name;
        }
    }

    public override string Description
    {
        get
        {
            return description;
        }
    }
}

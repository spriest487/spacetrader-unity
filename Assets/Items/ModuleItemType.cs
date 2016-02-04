using UnityEngine;

public class ModuleItemType : ItemType
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Create/SpaceTrader/Items/Equippable module item type")]
    public static void Create()
    {
        ScriptableObjectUtility.CreateAsset<ModuleItemType>();
    }
#endif

    [SerializeField]
    private ModuleDefinition moduleType;

    public override string Description
    {
        get
        {
            return moduleType.Description;
        }
    }

    public override string DisplayName
    {
        get
        {
            return moduleType.name;
        }
    }

    public ModuleDefinition ModuleDefinition
    {
        get
        {
            return moduleType;
        }
    }
}
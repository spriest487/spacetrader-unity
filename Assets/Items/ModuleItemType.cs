#pragma warning disable 0649

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
    private ModuleBehaviour behaviour;
    
    [TextArea]
    [SerializeField]
    private string flavorText;

    public ModuleBehaviour Behaviour { get { return behaviour; } }

    public override string Description
    {
        get
        {
            string result = "";

            //flavor text
            if (flavorText != null && flavorText.Length > 0)
            {
                result += flavorText + "\n";
            }

            //behaviour description (usually stats)
            return result + behaviour.Description;
        }
    }

    public override string DisplayName
    {
        get
        {
            return name;
        }
    }

    public string GetStatsString(Ship owner)
    {
        string result;

        var dps = behaviour.CalculateDps(owner);
        if (dps > 0)
        {
            result = string.Format("<size=32><color=#ffffffaa>DPS:</color> {0:0.0}</size>\n", dps);
        }
        else
        {
            result = "";
        }

        return result;
    }
}
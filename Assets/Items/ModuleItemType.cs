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

    [SerializeField]
    private float cooldownLength;

    [TextArea]
    [SerializeField]
    private string flavorText;

    public ModuleBehaviour Behaviour { get { return behaviour; } }
    public float CooldownLength { get { return cooldownLength; } }

    public override string Description
    {
        get
        {
            string result;

            //DPS
            var weapon = behaviour as IWeapon;
            if (weapon != null)
            {
                float dps = weapon.ApproxDamagePerActivation / cooldownLength;

                result = string.Format("<size=32><color=#ffffffaa>DPS:</color> {0:0.0}</size>\n", dps);
            }
            else
            {
                result = "";
            }

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
}
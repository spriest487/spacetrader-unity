using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ModuleDefinition : ScriptableObject
{
#if UNITY_EDITOR
    [MenuItem("Assets/Create/SpaceTrader/Modules/Module Definition")]
    public static void CreateModuleDefinition()
    {
        ScriptableObjectUtility.CreateAsset<ModuleDefinition>();
    }
#endif
    
	[SerializeField]
	private ModuleBehaviour behaviour;

	[SerializeField]
	private float cooldownLength;

    [TextArea]
    [SerializeField]
    private string description;
    
	public ModuleBehaviour Behaviour { get { return behaviour; } }
	public float CooldownLength { get { return cooldownLength; } }

    public string Description
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

            //description
            if (description != null && description.Length > 0)
            {
                result += description + "\n";
            }

            //behaviour description (usually stats)
            return result + behaviour.Description;
        }
    }

	public static ModuleDefinition Create(string name,
        ModuleBehaviour behaviour,
        float cooldownLength)
	{
        ModuleDefinition result = CreateInstance<ModuleDefinition>();
        
        result.behaviour = behaviour;
        result.cooldownLength = cooldownLength;

        return result;
	}
}
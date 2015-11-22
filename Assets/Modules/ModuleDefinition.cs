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
	private string moduleName;

	[SerializeField]
	private ModuleBehaviour behaviour;

	[SerializeField]
	private float cooldownLength;

	public string Name { get { return moduleName;  } }
	public ModuleBehaviour Behaviour { get { return behaviour; } }
	public float CooldownLength { get { return cooldownLength; } }

	public static ModuleDefinition Create(string name,
        ModuleBehaviour behaviour,
        float cooldownLength)
	{
        ModuleDefinition result = CreateInstance<ModuleDefinition>();

        result.moduleName = name;
        result.behaviour = behaviour;
        result.cooldownLength = cooldownLength;

        return result;
	}
}
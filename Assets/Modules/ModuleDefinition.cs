using UnityEngine;
using System.Collections.Generic;

public class ModuleDefinition : ScriptableObject
{
	[SerializeField]
	private string moduleName;

	[SerializeField]
	private ModuleBehaviour behaviour;

	[SerializeField]
	private float cooldownLength;

	public string Name { get { return moduleName;  } }
	public ModuleBehaviour Behaviour { get { return behaviour; } }
	public float CooldownLength { get { return cooldownLength; } }

	public ModuleDefinition(string name, ModuleBehaviour behaviour, float cooldownLength)
	{
		this.moduleName = name;
        this.behaviour = behaviour;
        this.cooldownLength = cooldownLength;
	}
}
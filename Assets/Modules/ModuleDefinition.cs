using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ModuleDefinition
{
	[SerializeField]
	private string _name;

	[SerializeField]
	private ModuleBehaviour _behaviour;

	[SerializeField]
	private float _cooldownLength;

	public string name { get { return _name;  } }
	public ModuleBehaviour behaviour { get { return _behaviour; } }
	public float cooldownLength { get { return _cooldownLength; } }

	public ModuleDefinition(string name, ModuleBehaviour behaviour, float cooldownLength)
	{
		_name = name;
		_behaviour = behaviour;
		_cooldownLength = cooldownLength;
	}
}
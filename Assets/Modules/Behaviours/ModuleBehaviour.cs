using UnityEngine;

public abstract class ModuleBehaviour : ScriptableObject
{
	public abstract void Activate(Ship activator, WeaponHardpoint hardpoint);
}
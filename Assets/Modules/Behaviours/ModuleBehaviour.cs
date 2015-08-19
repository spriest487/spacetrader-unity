using UnityEngine;

public abstract class ModuleBehaviour : ScriptableObject
{
	public abstract void Activate(Ship activator, WeaponHardpoint hardpoint, ModuleStatus module);
    
    public virtual Vector3? PredictTarget(Ship activator, WeaponHardpoint hardpoint, Targetable target)
    {
        return null;
    }
}
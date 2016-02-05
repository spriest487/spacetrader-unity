using UnityEngine;

public abstract class ModuleBehaviour : ScriptableObject
{
    public abstract string Description { get; }

	public abstract void Activate(Ship activator, int slot);
    
    public virtual Vector3? PredictTarget(Ship activator, int slot, Targetable target)
    {
        return null;
    }
}
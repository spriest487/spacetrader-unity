using UnityEngine;

public abstract class ModuleBehaviour : ScriptableObject
{
    [SerializeField]
    private float cooldown;

    public abstract string Description { get; }

    public abstract void Equip(HardpointModule slot);

	public abstract void Activate(Ship activator, int slot);

    public virtual float Cooldown
    {
        get { return cooldown; }
        set { cooldown = value; }
    }

    public void UpdateForOwner(Ship owner)
    {
        cooldown -= Time.deltaTime;
    }
    
    public virtual Vector3? PredictTarget(Ship activator, int slot, Targetable target)
    {
        return null;
    }
}
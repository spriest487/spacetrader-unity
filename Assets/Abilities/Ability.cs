using UnityEngine;

public abstract class Ability : ScriptableObject
{
    [SerializeField]
    private Sprite icon;

    [SerializeField]
    private float cooldown = 0;

    public Sprite Icon
    {
        get { return icon; }
    }

    public float Cooldown
    {
        get { return cooldown; }
        set { cooldown = value; }
    }

    public virtual bool Cancellable
    {
        get { return false; }
    }

    public abstract void Use(Ship ship);

    public virtual void Cancel(Ship ship)
    {
    }

    public abstract string Describe();
}
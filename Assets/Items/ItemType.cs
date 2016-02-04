using UnityEngine;

public abstract class ItemType : ScriptableObject
{
    [SerializeField]
    private int baseValue;

    [SerializeField]
    private Sprite icon;

    public abstract string DisplayName { get; }

    public abstract string Description { get; }

    public int BaseValue
    {
        get
        {
            return baseValue;
        }
    }

    public Sprite Icon
    {
        get
        {
            return icon;
        }
    }
}
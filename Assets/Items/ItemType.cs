using UnityEngine;

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    SuperRare
}

public abstract class ItemType : ScriptableObject
{
    [SerializeField]
    private int baseValue;

    [SerializeField]
    private Sprite icon;

    [SerializeField]
    private Rarity rarity;
   
    public abstract string DisplayName { get; }
    public abstract string Description { get; }

    public Rarity Rarity
    {
        get
        {
            return rarity;
        }
    }

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
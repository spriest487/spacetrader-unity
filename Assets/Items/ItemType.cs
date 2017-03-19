#pragma warning disable 0649

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

    public virtual IEnumerable<KeyValuePair<string, string>> GetDisplayedStats(Ship owner)
    {
        return Enumerable.Empty<KeyValuePair<string, string>>();
    }

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

public static class ItemTypeUtility
{
    public static Color Color(this Rarity rarity)
    {
        return Universe.CargoItemConfiguration.RarityColor(rarity);
    }
}
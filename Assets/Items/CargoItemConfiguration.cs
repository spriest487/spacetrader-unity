#pragma warning disable 0649

using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "SpaceTrader/Items/Item Configuration")]
public class CargoItemConfiguration : ScriptableObject
{
    [SerializeField]
    private List<ItemType> itemTypes;

    [SerializeField]
    private Color commonColor;

    [SerializeField]
    private Color uncommonColor;

    [SerializeField]
    private Color rareColor;

    [SerializeField]
    private Color superRareColor;

    public IEnumerable<ItemType> ItemTypes
    {
        get { return itemTypes; }
    }

    public Color CommonColor { get { return commonColor; } }
    public Color UncommonColor { get { return uncommonColor; } }
    public Color RareColor { get { return rareColor; } }
    public Color SuperRareColor { get { return superRareColor; } }

    private Dictionary<string, ItemType> typesByName;
    private Dictionary<string, ItemType> TypesByName
    {
        get
        {
            typesByName = new Dictionary<string, ItemType>();

            foreach (ItemType type in itemTypes)
            {
                if (type != null)
                {
                    typesByName.Add(type.name, type);
                }
            }

            return typesByName;
        }
    }

    public Color RarityColor(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.SuperRare:
                return superRareColor;
            case Rarity.Rare:
                return rareColor;
            case Rarity.Uncommon:
                return uncommonColor;
            default:
                return commonColor;
        }
    }

    public ItemType FindType(string name)
    {
        Debug.Assert(TypesByName.ContainsKey(name), "missing item type: " + name);

        return typesByName[name];
    }
}
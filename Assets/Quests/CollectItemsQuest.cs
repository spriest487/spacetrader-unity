#pragma warning disable 0649
using System;
using UnityEngine;

public class CollectItemsQuest : QuestBehaviour
{
    [SerializeField]
    private string itemTypeName;

    [SerializeField]
    private int quantity;

    public override int XPReward
    {
        get
        {
            switch (GetItemType().Rarity)
            {
                default:
                    return 100;
                case Rarity.Uncommon:
                    return 200;
                case Rarity.Rare:
                    return 400;
                case Rarity.SuperRare:
                    return 800;
            }
        }
    }

    public override int MoneyReward
    {
        get
        {
            return quantity * (GetItemType().BaseValue / 2);
        }
    }

    private ItemType GetItemType()
    {
        return SpaceTraderConfig.CargoItemConfiguration.FindType(itemTypeName);
    }

    public override bool Done(Quest quest)
    {
        var owner = SpaceTraderConfig.QuestBoard.OwnerOf(quest);
        if (!owner)
        {
            return false;
        }
        
        var cargo = owner.Ship.Cargo;

        return cargo.ContainsItems(GetItemType(), quantity);
    }

    public override string Describe(Quest quest)
    {
        return string.Format("Bring {0} {1}s to {2}", quantity, itemTypeName, quest.Station.name);
    }

    public override void OnFinish(Quest quest)
    {
        //remove the required items from the player

        var owner = SpaceTraderConfig.QuestBoard.OwnerOf(quest);
        Debug.Assert(owner);

        var cargo = owner.Ship.Cargo;
        var itemType = GetItemType();

        int removed = 0;
        for (int slot = 0; slot < cargo.Size && removed < quantity; ++slot)
        {
            if (cargo[slot] == itemType)
            {
                cargo.RemoveAt(slot);
                ++removed;
            }
        }

        Debug.Assert(removed == quantity);
    }

    public override void OnAbandon(Quest quest)
    {
    }
}

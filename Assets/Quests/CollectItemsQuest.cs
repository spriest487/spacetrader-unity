#pragma warning disable 0649
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "SpaceTrader/Quests/Generic Fetch Quest")]
public class CollectItemsQuest : Quest
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
        return Universe.CargoItemConfiguration.FindType(itemTypeName);
    }

    public override QuestStatus Status
    {
        get
        {
            var owner = Universe.QuestBoard.OwnerOf(this);
            if (!owner)
            {
                return QuestStatus.NotAccepted;
            }

            var cargo = owner.Ship.Cargo;

            if (cargo.ContainsItems(GetItemType(), quantity))
            {
                return QuestStatus.Completed;
            }
            else
            {
                return QuestStatus.InProgress;
            }
        }
    }

    public override string Description
    {
        get
        {
            return string.Format("Bring {0} {1}s to {2} in {3}", quantity, itemTypeName, Location.Station, Location.Area);
        }
    }

    public override void OnFinish()
    {
        //remove the required items from the player

        var owner = Universe.QuestBoard.OwnerOf(this);
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
}

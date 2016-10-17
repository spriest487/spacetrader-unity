#pragma warning disable 0649

using UnityEngine;

[CreateAssetMenu(menuName = "SpaceTrader/Items/Cargo Item Type")]
public class CargoItemType : ItemType
{
    [SerializeField]
    private string displayName;

    [TextArea]
    [SerializeField]
    private string description;
    
    public override string DisplayName
    {
        get
        {
            if (displayName != null)
            {
                return displayName;
            }

            return name;
        }
    }

    public override string Description
    {
        get
        {
            return description;
        }
    }
}

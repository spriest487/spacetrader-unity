#pragma warning disable 0649

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SpaceTrader/Items/Equippable Item Type")]
public class ModuleItemType : ItemType
{
    [SerializeField]
    private ModuleBehaviour behaviour;
    
    [TextArea]
    [SerializeField]
    private string flavorText;

    public ModuleBehaviour Behaviour { get { return behaviour; } }

    public override string Description
    {
        get
        {
            string result = "";

            //flavor text
            if (flavorText != null && flavorText.Length > 0)
            {
                result += flavorText + "\n";
            }

            //behaviour description (usually stats)
            return result + behaviour.Description;
        }
    }

    public override string DisplayName
    {
        get
        {
            return name;
        }
    }

    public override IEnumerable<KeyValuePair<string, string>> GetDisplayedStats(Ship owner)
    {
        yield return new KeyValuePair<string, string>("DPS", behaviour.CalculateDps(owner).ToString("0.0"));

        foreach (var stat in behaviour.GetDisplayedStats(owner))
        {
            yield return stat;
        }
    }
}

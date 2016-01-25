using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShipModuleController : MonoBehaviour
{
    //[SerializeField]
    //private ModuleStatus moduleStatus;

    [SerializeField]
    private Text caption;

    [SerializeField]
    private Image icon;

    public static ShipModuleController CreateFromPrefab(ShipModuleController prefab,
        ModuleStatus module)
    {
        var result = Instantiate(prefab);

        var itemName = module.Definition.name;
        var itemType = SpaceTraderConfig.CargoItemConfiguration.FindType(itemName);

        result.caption.text = itemName;
        result.icon.sprite = itemType.Icon;

        return result;
    }
}

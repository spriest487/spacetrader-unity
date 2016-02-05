using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ShipModuleController : MonoBehaviour
{
    [SerializeField]
    private Text caption;

    [SerializeField]
    private Image icon;

    [SerializeField]
    private Ship ship;

    [SerializeField]
    private int moduleSlot;

    public Ship Ship
    {
        get { return ship; }
    }

    public int ModuleSlot
    {
        get { return moduleSlot; }
    }

    public ModuleStatus Module
    {
        get { return Ship.ModuleLoadout.HardpointModules[ModuleSlot]; }
    }

    public void OnClickModule()
    {
        SendMessageUpwards("OnSelectShipModule", this, SendMessageOptions.DontRequireReceiver);
    }

    public static ShipModuleController CreateFromPrefab(ShipModuleController prefab,
        Ship ship,
        int moduleIndex)
    {
        var result = Instantiate(prefab);

        result.moduleSlot = moduleIndex;
        result.ship = ship;

        var itemName = result.Module.ModuleType.name;
        var itemType = SpaceTraderConfig.CargoItemConfiguration.FindType(itemName);

        result.caption.text = itemName;
        result.icon.sprite = itemType.Icon;

        return result;
    }
}

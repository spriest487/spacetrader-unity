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
    private ModuleLoadout loadout;

    [SerializeField]
    private int moduleSlot;

    public ModuleLoadout ModuleLoadout
    {
        get { return loadout; }
    }

    public int ModuleSlot
    {
        get { return moduleSlot; }
    }

    public ModuleStatus Module
    {
        get { return ModuleLoadout.FrontModules[ModuleSlot]; }
    }

    public void OnClickModule()
    {
        SendMessageUpwards("OnSelectShipModule", this, SendMessageOptions.DontRequireReceiver);
    }

    public static ShipModuleController CreateFromPrefab(ShipModuleController prefab,
        ModuleLoadout loadout,
        int moduleIndex)
    {
        var result = Instantiate(prefab);

        result.moduleSlot = moduleIndex;
        result.loadout = loadout;

        var itemName = result.Module.Definition.name;
        var itemType = SpaceTraderConfig.CargoItemConfiguration.FindType(itemName);

        result.caption.text = itemName;
        result.icon.sprite = itemType.Icon;

        return result;
    }
}

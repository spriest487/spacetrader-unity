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
    private Transform highlight;

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

    public bool Highlighted
    {
        get { return highlight.gameObject.activeSelf; }
        set { highlight.gameObject.SetActive(value); }
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

        var itemType = result.Module.ModuleType;

        result.caption.text = itemType.name;
        result.icon.sprite = itemType.Icon;

        result.Highlighted = false;

        return result;
    }
}

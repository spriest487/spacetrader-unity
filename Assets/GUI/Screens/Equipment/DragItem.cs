#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

public class DragItem : MonoBehaviour
{
    [SerializeField]
    private CargoHoldListItem item;

    [SerializeField]
    private ShipModuleController module;

    [SerializeField]
    private Image icon;

    public CargoHoldListItem Item
    {
        get { return item; }
        set
        {
            item = value;
            if (item)
            {
                icon.sprite = item.ItemType.Icon;
            }
            else
            {
                icon.sprite = null;
            }

            Update();
        }
    }

    public ShipModuleController Module
    {
        get { return module; }
        set
        {
            module = value;
            if (module)
            {
                icon.sprite = module.Module.ModuleType.Icon;
            }
            else
            {
                icon.sprite = null;
            }

            Update();
        }
    }

    private void Update()
    {
        if (item || module)
        {
            transform.position = Input.mousePosition;
        }
    }
}

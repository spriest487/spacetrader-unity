#pragma warning disable 0649

using UnityEngine;

public class ShipModulesController : MonoBehaviour
{
    private PooledList<ShipModuleController, HardpointModule> modules;

    [SerializeField]
    private ShipModuleController moduleTemplate;

    [SerializeField]
    private Transform modulesRoot;

    [SerializeField]
    private int highlightedIndex = CargoHold.BadIndex;

    public ShipModuleController this[int index]
    {
        get
        {
            Debug.Assert(modules != null);
            return modules[index];
        }
    }

    public int HighlightedIndex
    {
        get { return highlightedIndex; }
        set
        {
            if (modules == null)
            {
                Update();
            }

            if (!Universe.LocalPlayer || !Universe.LocalPlayer.Ship.ModuleLoadout.IsValidSlot(value))
            {
                value = -1;
            }

            highlightedIndex = value;

            for (int modIndex = 0; modIndex < modules.Count; ++modIndex)
            {
                modules[modIndex].Highlighted = modIndex == value;
            }
        }
    }

    public ShipModuleController HighlightedModule
    {
        get
        {
            if (modules == null || !Universe.LocalPlayer.Ship.ModuleLoadout.IsValidSlot(highlightedIndex))
            {
                return null;
            }

            return modules[highlightedIndex];
        }
    }

    private void Update()
    {
        var player = Universe.LocalPlayer;

        if (modules == null)
        {
            modules = new PooledList<ShipModuleController, HardpointModule>(modulesRoot, moduleTemplate);
        }

        if (!player)
        {
            modules.Clear();
            return;
        }

        modules.Refresh(player.Ship.ModuleLoadout, (i, module, slot) =>
            module.Assign(player.Ship, i, i == highlightedIndex));
    }

    public void Refresh()
    {
        Update();
    }

    private void OnSelectShipModule(ShipModuleController selected)
    {
        for (int moduleIndex = 0; moduleIndex < modules.Count; ++moduleIndex)
        {
            var module = modules[moduleIndex];
            if (module == selected)
            {
                HighlightedIndex = moduleIndex;
                break;
            }
        }
    }
}

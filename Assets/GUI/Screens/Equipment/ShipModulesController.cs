#pragma warning disable 0649

using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(UnityEngine.UI.LayoutGroup))]
public class ShipModulesController : MonoBehaviour
{
    private PooledList<ShipModuleController, HardpointModule> modules;
    
    [SerializeField]
    private ShipModuleController moduleTemplate;

    [SerializeField]
    private int highlightedIndex = -1;

    public int HighlightedIndex
    {
        get { return highlightedIndex; }
        set
        {
            highlightedIndex = value;

            var modules = GetComponentsInChildren<ShipModuleController>();
            for (int modIndex = 0; modIndex < modules.Length; ++modIndex)
            {
                modules[modIndex].Highlighted = modIndex == value;
            }
        }
    }
    
    private void Update()
    {
        var player = PlayerShip.LocalPlayer;

        if (modules == null)
        {
            modules = new PooledList<ShipModuleController, HardpointModule>(transform);
        }

        if (!player)
        {
            modules.Clear();
            return;
        }

        var slots = new List<HardpointModule>(player.Ship.ModuleLoadout);
        
        modules.Refresh(slots,
            (i, slot) => ShipModuleController.CreateFromPrefab(moduleTemplate, player.Ship, i),
            (i, module, slot) => { module.Assign(player.Ship, i); });
    }

    public void Refresh()
    {
        Update();
    }

    private void OnSelectShipModule(ShipModuleController selected)
    {
        var modules = GetComponentsInChildren<ShipModuleController>();
        
        for (int moduleIndex = 0; moduleIndex < modules.Length; ++moduleIndex)
        {
            var module = modules[moduleIndex];
            if (module == selected)
            {
                highlightedIndex = moduleIndex;
                module.Highlighted = true;
            }
            else
            {
                module.Highlighted = false;                
            }
        }
    }
}

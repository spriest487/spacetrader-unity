#pragma warning disable 0649

using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(UnityEngine.UI.LayoutGroup))]
public class ShipModulesController : MonoBehaviour
{
    private List<ModuleStatus> lastModules;
    
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

    private void Clear()
    {
        lastModules = null;
        foreach (var oldModule in GetComponentsInChildren<ShipModuleController>())
        {
            Destroy(oldModule.gameObject);
        }
    }

    private void Update()
    {
        var player = PlayerShip.LocalPlayer;

        if (!player)
        {
            Clear();
            return;
        }

        var currentModules = new List<ModuleStatus>();
        foreach (var module in player.Ship.ModuleLoadout.HardpointModules)
        {
            currentModules.Add(module);
        }
        
        bool needsRefresh = lastModules == null || !currentModules.ElementsEquals(lastModules);

        if (needsRefresh)
        {
            Clear();

            var moduleCount = currentModules.Count;
            for (int moduleSlot = 0; moduleSlot < moduleCount; ++moduleSlot)
            {
                var module = ShipModuleController.CreateFromPrefab(moduleTemplate, player.Ship, moduleSlot);
                module.transform.SetParent(transform, false);
            }

            lastModules = currentModules;
        }
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

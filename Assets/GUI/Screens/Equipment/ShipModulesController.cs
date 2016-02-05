using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

[RequireComponent(typeof(UnityEngine.UI.LayoutGroup))]
public class ShipModulesController : MonoBehaviour
{
    private List<ModuleStatus> lastModules;
    
    [SerializeField]
    private ShipModuleController moduleTemplate;

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
}

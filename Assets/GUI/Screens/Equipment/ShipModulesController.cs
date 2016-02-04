using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

[RequireComponent(typeof(UnityEngine.UI.LayoutGroup))]
public class ShipModulesController : MonoBehaviour
{
    public enum Target
    {
        FRONT,
        BACK
    }

    private List<ModuleStatus> lastModules;

    [SerializeField]
    private Target target;

    [SerializeField]
    private ShipModuleController moduleTemplate;

    private void Update()
    {
        var player = PlayerShip.LocalPlayer;

        if (!player)
        {
            return;
        }

        var loadout = player.GetComponent<ModuleLoadout>();
        if (loadout == null)
        {
            return;
        }

        List<ModuleStatus> currentModules = new List<ModuleStatus>();
        var group = Target.FRONT.Equals(target) ? loadout.FrontModules : loadout.FrontModules;

        foreach (var module in group)
        {
            currentModules.Add(module);
        }

        bool needsRefresh;
        if (lastModules != null)
        {
            var matching = lastModules.FindAll(m => currentModules.Contains(m));

            needsRefresh = matching.Count != currentModules.Count;
        }
        else
        {
            needsRefresh = true;
        }

        lastModules = currentModules;

        if (needsRefresh)
        {
            foreach (var oldModule in GetComponentsInChildren<ShipModuleController>())
            {
                Destroy(oldModule.gameObject);
            }

            var moduleCount = currentModules.Count;
            for (int moduleSlot = 0; moduleSlot < moduleCount; ++moduleSlot)
            {
                var module = ShipModuleController.CreateFromPrefab(moduleTemplate, loadout, moduleSlot);
                module.transform.SetParent(transform, false);
            }
        }
    }
}

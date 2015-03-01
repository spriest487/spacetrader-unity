using UnityEngine;
using System.Collections.Generic;

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

    void Update()
    {
        var player = PlayerStart.ActivePlayer;

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

            foreach (var newModule in currentModules)
            {
                var module = (ShipModuleController) Instantiate(moduleTemplate);
                module.transform.parent = this.transform;

                module.Module = newModule;
            }
        }
    }
}

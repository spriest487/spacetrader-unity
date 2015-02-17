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

        bool needsRefresh;
        if (lastModules != null)
        {
            var currentModules = new List<ModuleStatus>();

            var group = Target.FRONT.Equals(target)? loadout.FrontModules : loadout.FrontModules;

            foreach (var module in group)
            {
                currentModules.Add(module);
            }

            needsRefresh = !currentModules.Equals(lastModules);
        }
        else
        {
            lastModules = new List<ModuleStatus>();
            needsRefresh = true;
        }

        if (needsRefresh)
        {
            foreach (var oldModule in GetComponentsInChildren<ShipModuleController>())
            {
                Destroy(oldModule.gameObject);
            }

            foreach (var newModule in lastModules)
            {
                var module = (ShipModuleController) Instantiate(moduleTemplate);
                module.transform.parent = this.transform;
            }
        }
    }
}

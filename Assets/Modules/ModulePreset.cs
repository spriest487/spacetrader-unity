using UnityEngine;

public class ModulePreset : ScriptableObject
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Create/SpaceTrader/Modules/Module Preset")]
    public static void CreateModulePreset()
    {
        ScriptableObjectUtility.CreateAsset<ModulePreset>();
    }
#endif

    [SerializeField]
    string[] frontModules;

    [SerializeField]
    string[] cargoItems;

    public string[] FrontModules { get { return frontModules; } }
    public string[] CargoItems { get { return cargoItems; } }

    public void Apply(ModuleLoadout moduleLoadout)
    {
        moduleLoadout.FrontModules.Resize(frontModules.Length);
        for (int module = 0; module < frontModules.Length; ++module)
        {
            var moduleName = frontModules[module];
            var itemType = SpaceTraderConfig.CargoItemConfiguration.FindType(moduleName);

            if (itemType == null || itemType.ModuleDefinition == null)
            {
                throw new UnityException("bad module item name: " +moduleName);
            }
            moduleLoadout.FrontModules.Equip(module, itemType.ModuleDefinition);
        }

        var cargo = moduleLoadout.GetComponent<Ship>().Cargo;

        if (cargo)
        {
            foreach (var item in cargoItems)
            {
                cargo.Add(item);
            }
        }
    }
}
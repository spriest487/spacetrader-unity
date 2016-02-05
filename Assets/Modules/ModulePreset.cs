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
    private string[] frontModules;

    [SerializeField]
    private string[] cargoItems;

    public string[] FrontModules { get { return frontModules; } }
    public string[] CargoItems { get { return cargoItems; } }

    public void Apply(Ship ship)
    {
        var moduleLoadout = ship.ModuleLoadout;
        moduleLoadout.HardpointModules.Resize(frontModules.Length);

        for (int module = 0; module < frontModules.Length; ++module)
        {
            var moduleName = frontModules[module];
            var itemType = SpaceTraderConfig.CargoItemConfiguration.FindType(moduleName) as ModuleItemType;

            Debug.Assert(itemType != null, "bad module item name: " +moduleName);
            
            moduleLoadout.Equip(module, itemType);
        }
        
        foreach (var item in cargoItems)
        {
            ship.Cargo.Add(item);
        }
    }
}
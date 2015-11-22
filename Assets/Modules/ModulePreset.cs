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
            moduleLoadout.FrontModules.Equip(module, frontModules[module]);
        }

        var cargo = moduleLoadout.GetComponent<CargoHold>();

        if (cargo)
        {
            cargo.Items = cargoItems;
        }
    }
}
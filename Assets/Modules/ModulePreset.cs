using UnityEngine;

public class ModulePreset : ScriptableObject
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Create/SpaceTrader/Module Preset")]
    public static void CreateModulePreset()
    {
        ScriptableObjectUtility.CreateAsset<ModulePreset>();
    }
#endif

    [SerializeField]
    private string[] frontModules;

    public string[] FrontModules
    {
        get { return frontModules; }
    }

    public void Apply(ModuleLoadout moduleLoadout)
    {
        moduleLoadout.FrontModules.Resize(frontModules.Length);
        for (int module = 0; module < frontModules.Length; ++module)
        {
            moduleLoadout.FrontModules.Equip(module, frontModules[module]);
        }
    }
}
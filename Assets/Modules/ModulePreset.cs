using UnityEngine;

[RequireComponent(typeof(ModuleLoadout))]
public class ModulePreset : MonoBehaviour
{
    public string[] frontModules;

    void Start()
    {
        var moduleLoadout = GetComponent<ModuleLoadout>();

        moduleLoadout.FrontModules.Resize(frontModules.Length);
        for (int module = 0; module < frontModules.Length; ++module)
        {
            moduleLoadout.FrontModules.Equip(module, frontModules[module]);
        }
    }
}
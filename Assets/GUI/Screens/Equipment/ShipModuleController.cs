using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShipModuleController : MonoBehaviour
{
    [SerializeField]
    private ModuleStatus moduleStatus;

    [SerializeField]
    private Text caption;

    public ModuleStatus Module {
        get
        {
            return moduleStatus;
        }
        set
        {
            moduleStatus = value;
        }
    }

    void Update()
    {
        if (caption && moduleStatus)
        {
            caption.text = moduleStatus.Definition.Name;
        }
    }
}

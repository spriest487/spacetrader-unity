using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShipModuleController : MonoBehaviour
{
    [SerializeField]
    private ModuleStatus moduleStatus;

    [SerializeField]
    private Text caption;

    public ModuleStatus Module { get; set; }

    void Start()
    {
        var captionObj = transform.FindChild("Caption");
        if (captionObj)
        {
            caption = captionObj.GetComponent<Text>();
        }
    }

    void Update()
    {
        if (caption)
        {
            caption.text = moduleStatus.Definition.Name;
        }
    }
}

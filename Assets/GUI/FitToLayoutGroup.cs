#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(LayoutElement))]
public class FitToLayoutGroup : MonoBehaviour
{
    [SerializeField]
    private LayoutGroup target;

    private LayoutElement layout;

    void Start()
    {
        layout = GetComponent<LayoutElement>();
    }

    void Update()
    {
#if UNITY_EDITOR
        if (!target) return;
#endif
        
        layout.preferredHeight = target.preferredHeight;
        layout.preferredWidth = target.preferredWidth;
    }
}

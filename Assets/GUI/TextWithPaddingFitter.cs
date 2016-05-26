#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(LayoutElement))]
public class TextWithPaddingFitter : MonoBehaviour
{
    [SerializeField]
    private Text text;

    [SerializeField]
    private int padding = 0;
    
    private LayoutElement layout;

    private void Start()
    {
        layout = GetComponent<LayoutElement>();
    }

    private void Update()
    {
        var height = text.preferredHeight + padding;

        layout.preferredHeight = height;
    }
}

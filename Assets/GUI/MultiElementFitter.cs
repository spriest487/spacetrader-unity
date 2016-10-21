#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class MultiElementFitter : MonoBehaviour
{
    [SerializeField]
    private List<RectTransform> targets;

    [SerializeField]
    private ContentSizeFitter.FitMode fitMode = ContentSizeFitter.FitMode.PreferredSize;

    [SerializeField]
    private int extraPadding = 0;
    
    private LayoutElement layout;

    public IEnumerable<RectTransform> Elements
    {
        get { return targets; }
        set { targets = new List<RectTransform>(value); }
    }

    private void Start()
    {
        layout = GetComponent<LayoutElement>();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (targets == null)
        {
            targets = new List<RectTransform>();
        }

        Start();
#endif

        float total = 0;
        foreach (var element in targets)
        {
            total += LayoutUtility.GetPreferredHeight(element);
        }

        int size = extraPadding + Mathf.CeilToInt(total);

        if (layout)
        {
            switch (fitMode)
            {
                case ContentSizeFitter.FitMode.MinSize:
                    layout.minHeight = size;
                    break;
                case ContentSizeFitter.FitMode.PreferredSize:
                    layout.preferredHeight = size;
                    break;
            }
        }
        else
        {
            var xform = (RectTransform)transform;
            var sizeDelta = xform.sizeDelta;
            sizeDelta.y = size;
            xform.sizeDelta = sizeDelta;
        }
    }
}
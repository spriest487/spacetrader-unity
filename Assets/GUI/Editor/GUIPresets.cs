using UnityEditor;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using System;

public static class GUIPresets
{
    private static void ConfigureButton(Button button)
    {
        if (!button)
        {
            return;
        }

        var font = AssetDatabase.LoadAssetAtPath<Font>("Assets/GUI/Coffee.ttf");

        var label = button.GetComponentInChildren<Text>();
        if (label)
        {
            label.font = font;
            label.fontSize = 32;
            label.color = Color.white;
            label.raycastTarget = false;
            label.supportRichText = false;
            label.horizontalOverflow = HorizontalWrapMode.Overflow;
            label.verticalOverflow = VerticalWrapMode.Overflow;

            if (!label.GetComponent<Outline>())
            {
                var outline = label.gameObject.AddComponent<Outline>();
                outline.effectColor = Color.black;
            }
        }

        var bg = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/GUI/Backing Blur.psd");
        var bgImage = button.GetComponent<Image>();
        bgImage.type = Image.Type.Simple;
        bgImage.sprite = bg;
        button.targetGraphic = label;

        var colors = button.colors;
        colors.normalColor = new Color(1, 0.21f, 0.39f);
        colors.highlightedColor = new Color(1, 0.74f, 0.8f);
        colors.pressedColor = Color.white;
        colors.disabledColor = new Color(1, 0.21f, 0.39f, 0.5f);

        button.colors = colors;

        var layout = button.GetComponent<LayoutElement>();
        if (layout)
        {
            layout.preferredHeight = 48;
        }
    }

    private static void ForSelected<T>(Action<T> action)
        where T : UnityEngine.Object
    {
        var selection = Selection.activeGameObject;
        if (!selection)
        {
            return;
        }

        foreach (var obj in Selection.gameObjects
            .Select(o => o.GetComponent<T>())
            .Where(obj => obj))
        {
            action(obj);
        }
    }

    [MenuItem("SpaceTrader/Apply Button Preset")]
    public static void ConfigureSelectedButtons()
    {
        ForSelected<Button>(ConfigureButton);
    }

    private static void ConfigurePanel(Image panel)
    {
        if (!panel)
        {
            return;
        }

        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/GUI/Element Square Mask.psd");

        panel.sprite = sprite;
        panel.type = Image.Type.Sliced;

        var cols = new Vector3(60, 60, 60) / 255;
        var a = 220.0f / 255;
        panel.color = new Color(cols.x, cols.y, cols.z, a);
    }

    [MenuItem("SpaceTrader/Apply Panel Preset")]
    public static void ConfigureSelectedPanels()
    {
        ForSelected<Image>(ConfigurePanel);
    }
}

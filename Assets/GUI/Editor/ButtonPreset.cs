using UnityEditor;
using UnityEngine.UI;
using UnityEngine;

public class ButtonPreset
{
    [MenuItem("SpaceTrader/Apply button preset")]
    public static void ConfigureButton()
    {
        var selection = Selection.activeGameObject;
        if (!selection)
        {
            return;
        }

        var button = selection.GetComponent<Button>();
        if (!button)
        {
            return;
        }

        var font = AssetDatabase.LoadAssetAtPath<Font>("Assets/GUI/Coffee.ttf");

        var label = button.GetComponentInChildren<Text>();
        if (label)
        {
            label.font = font;
            label.fontSize = 24;
            label.color = Color.white;

            if (!label.GetComponent<Outline>())
            {
                var outline = label.gameObject.AddComponent<Outline>();
                outline.effectColor = Color.black;
            }
        }

        var bg = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/GUI/Backing Blur.psd");
        button.GetComponent<Image>().sprite = bg;
        button.targetGraphic = label;

        var colors = button.colors;
        colors.normalColor = new Color(1, 0.21f, 0.39f);
        colors.highlightedColor = new Color(1, 0.74f, 0.8f);
        colors.pressedColor = Color.white;
        colors.disabledColor = new Color(1, 0.21f, 0.39f, 0.5f);

        button.colors = colors;
    }
}

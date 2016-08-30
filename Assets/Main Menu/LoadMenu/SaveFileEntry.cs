using UnityEngine;
using UnityEngine.UI;

public class SaveFileEntry : Toggle {

    [SerializeField]
    private Text entryText;

    private ColorBlock startColors;
    private Sprite startSprite;

    protected override void Awake()
    {
        entryText = GetComponentInChildren<Text>();

        if (entryText == null)
            Debug.LogWarning("SaveFileEntry: missing text reference");

        startColors = colors;
        startSprite = image.sprite;

        base.Awake();
    }

    public void Init()
    {
        onValueChanged.RemoveAllListeners();

        isOn = false;

        startSprite = ((Image)targetGraphic).sprite;
        onValueChanged.AddListener(value =>
        {
            var newColors = colors;

            switch (transition)
            {
                case Transition.ColorTint:
                    newColors.normalColor = isOn ? colors.pressedColor : startColors.normalColor;
                    newColors.highlightedColor = isOn ? colors.pressedColor : startColors.highlightedColor;
                    break;
                case Transition.SpriteSwap:
                    image.sprite = isOn ? spriteState.pressedSprite : startSprite; // untested
                    break;
            }

            colors = newColors;
        });
    }

    public void SetText(string text)
    {
        if (entryText != null)
        entryText.text = text;
    }
}

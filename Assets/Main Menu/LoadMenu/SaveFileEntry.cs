using UnityEngine;
using UnityEngine.UI;

public class SaveFileEntry : Toggle {

    [SerializeField]
    private Text entryText;

    private Sprite normalSprite;

    protected override void Awake()
    {
        entryText = GetComponentInChildren<Text>();
        if (entryText == null)
            Debug.LogWarning("SaveFileEntry: missing text reference");

        base.Awake();
    }

    public void Init()
    {
        onValueChanged.RemoveAllListeners();

        image.color = colors.normalColor;
        image.sprite = image.sprite = normalSprite;

        isOn = false;

        normalSprite = ((Image)targetGraphic).sprite;
        onValueChanged.AddListener(value =>
        {
            switch (transition)
            {
                case Transition.ColorTint: image.color = isOn ? colors.pressedColor : colors.normalColor; break;
                case Transition.SpriteSwap: image.sprite = isOn ? spriteState.pressedSprite : normalSprite; break;
            }
        });
    }

    public void SetText(string text)
    {
        if (entryText != null)
        entryText.text = text;
    }
}

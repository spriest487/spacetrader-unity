#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

public class ItemInformationPanel : MonoBehaviour
{
    [SerializeField]
    private ItemType itemType;

    [SerializeField]
    private Text nameLabel;

    [SerializeField]
    private Text descriptionLabel;

    [SerializeField]
    private Image icon;

    [SerializeField]
    private Sprite emptyIcon;

    public ItemType ItemType
    {
        get
        {
            return itemType;
        }
        set
        {
            itemType = value;
        }
    }

    private void Update()
    {
        if (itemType)
        {
            nameLabel.text = itemType.DisplayName;
            icon.sprite = itemType.Icon;
            descriptionLabel.text = itemType.Description;
        }
        else
        {
            nameLabel.text = "No item selected";
            icon.sprite = emptyIcon;
            descriptionLabel.text = null;
        }
    }
}

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

    private ScrollRect scrollRect;

    public ItemType ItemType
    {
        get
        {
            return itemType;
        }
        set
        {
            itemType = value;

            scrollRect.normalizedPosition = new Vector2(0, 1);
        }
    }

    private void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    private void Update()
    {
        if (itemType)
        {
            nameLabel.text = itemType.DisplayName;
            icon.sprite = itemType.Icon;
            icon.gameObject.SetActive(true);

            var moduleType = itemType as ModuleItemType;
            if (moduleType != null)
            {
                descriptionLabel.text = moduleType.GetStatsString(PlayerShip.LocalPlayer.Ship) + "\n" + itemType.Description;
            }
            else
            {
                descriptionLabel.text = itemType.Description;
            }
        }
        else
        {
            nameLabel.text = "No item selected";
            icon.sprite = emptyIcon;
            icon.gameObject.SetActive(false);
            descriptionLabel.text = null;
        }
    }
}

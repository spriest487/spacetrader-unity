#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

public class ItemInformationPanel : MonoBehaviour
{
    [SerializeField]
    private ItemType itemType;

    [SerializeField]
    private bool itemOwnedByPlayer;

    [SerializeField]
    private Text nameLabel;

    [SerializeField]
    private Text descriptionLabel;

    [SerializeField]
    private Image icon;

    [SerializeField]
    private Sprite emptyIcon;

    private ScrollRect scrollRect;
    
    public void SetItem(ItemType type, bool ownedByPlayer)
    {
        itemType = type;
        itemOwnedByPlayer = ownedByPlayer;
        Refresh();
    }

    private void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    private void Refresh()
    {
        if (itemType)
        {
            nameLabel.text = itemType.DisplayName;
            icon.sprite = itemType.Icon;
            icon.gameObject.SetActive(true);

            var description = "<size=32><color=#ffffffaa>Price:</color>"
                + Market.FormatCurrency(itemType.BaseValue)
                + "</size>\n";

            var moduleType = itemType as ModuleItemType;
            if (moduleType != null)
            {
                description += moduleType.GetStatsString(PlayerShip.LocalPlayer.Ship) + "\n" + itemType.Description;
            }
            else
            {
                description += itemType.Description;
            }

            descriptionLabel.text = description;
        }
        else
        {
            nameLabel.text = "No item selected";
            icon.sprite = emptyIcon;
            icon.gameObject.SetActive(false);
            descriptionLabel.text = null;
        }
    }

    private void OnScreenActive()
    {
        Refresh();
    }
}

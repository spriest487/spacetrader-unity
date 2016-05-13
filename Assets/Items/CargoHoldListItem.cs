#pragma warning disable 0649

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CargoHoldListItem : MonoBehaviour
{
    [SerializeField]
    private Text label;

    [SerializeField]
    private Text priceLabel;

    [SerializeField]
    private Image icon;

    [SerializeField]
    private Transform highlight;

    [SerializeField]
    private CargoHold cargoHold;

    [SerializeField]
    private int itemIndex;

    public CargoHold CargoHold
    {
        get
        {
            return cargoHold;
        }
    }

    public int ItemIndex
    {
        get
        {
            return itemIndex;
        }
    }

    public ItemType Item
    {
        get
        {
            return CargoHold[ItemIndex];
        }
    }

    public bool Highlighted
    {
        get
        {
            return highlight.gameObject.activeSelf;
        }
        set
        {
            highlight.gameObject.SetActive(value);
        }
    }

    public void OnClickCargoItem()
    {
        SendMessageUpwards("OnSelectCargoItem", this, SendMessageOptions.DontRequireReceiver);
    }

    public void Drag()
    {
        SendMessageUpwards("OnDragCargoItem", this, SendMessageOptions.DontRequireReceiver);
    }
    
    public static CargoHoldListItem CreateFromPrefab(CargoHoldListItem prefab,
        CargoHold cargoHold,
        int itemIndex)
    {
        var result = Instantiate(prefab);
        result.cargoHold = cargoHold;
        result.itemIndex = itemIndex;

        if (cargoHold.IsIndexFree(itemIndex))
        {
            result.icon.gameObject.SetActive(false);
            result.label.gameObject.SetActive(false);

            if (result.priceLabel)
            {
                result.priceLabel.gameObject.SetActive(false);
            }
        }
        else
        {
            var itemType = cargoHold[itemIndex];

            result.label.text = itemType.DisplayName;
            result.icon.sprite = itemType.Icon;

            if (result.priceLabel)
            {
                result.priceLabel.text = Market.FormatCurrency(itemType.BaseValue);
            }
        }

        return result;
    }
}

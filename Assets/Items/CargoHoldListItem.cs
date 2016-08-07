#pragma warning disable 0649

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(Button))]
public class CargoHoldListItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField]
    private Text priceLabel;

    [SerializeField]
    private Image icon;

    [SerializeField]
    private Transform highlight;

    [SerializeField]
    private CargoHold cargoHold;

    [SerializeField]
    private Image rarityHighlight;

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

    public ItemType ItemType
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

    public void OnDrag(PointerEventData pointerData)
    {
    }

    public void OnBeginDrag(PointerEventData pointerData)
    {
        if (!cargoHold.IsIndexFree(itemIndex))
        {
            //Debug.Log("dragged from slot " + itemIndex);
            SendMessageUpwards("OnDragCargoItem", this, SendMessageOptions.DontRequireReceiver);
        }
    }

    public void OnEndDrag(PointerEventData pointerData)
    {
        if (!cargoHold.IsIndexFree(itemIndex))
        {
            var hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, hits);

            if (hits.Count != 0)
            {
                hits[0].gameObject.SendMessage("OnDropCargoItem", this, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    public void Assign(CargoHold cargoHold, int itemIndex)
    {
        this.cargoHold = cargoHold;
        this.itemIndex = itemIndex;

        if (cargoHold.IsIndexFree(itemIndex))
        {
            icon.gameObject.SetActive(false);
            //result.label.gameObject.SetActive(false);

            if (priceLabel)
            {
                priceLabel.gameObject.SetActive(false);
            }

            rarityHighlight.gameObject.SetActive(false);
        }
        else
        {
            var itemType = cargoHold[itemIndex];

            //result.label.text = itemType.DisplayName;
            icon.sprite = itemType.Icon;
            icon.gameObject.SetActive(true);

            if (priceLabel)
            {
                priceLabel.text = Market.FormatCurrency(itemType.BaseValue);
                priceLabel.gameObject.SetActive(true);
            }

            rarityHighlight.gameObject.SetActive(true);
            rarityHighlight.color = SpaceTraderConfig.CargoItemConfiguration.RarityColor(itemType.Rarity);
        }
    }
    
    public static CargoHoldListItem CreateFromPrefab(CargoHoldListItem prefab,
        CargoHold cargoHold,
        int itemIndex)
    {
        var result = Instantiate(prefab);
        result.Assign(cargoHold, itemIndex);

        return result;
    }
}

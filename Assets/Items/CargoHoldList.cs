using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

public class CargoHoldList : MonoBehaviour
{
    [SerializeField]
    private CargoHold cargoHold;

    [SerializeField]
    private int highlightedIndex;

    [SerializeField]
    private CargoHoldListItem listItem;

    [SerializeField]
    private Transform itemsHolder;

    [SerializeField]
    private string sizeFormat = "{0}/{1}";

    [SerializeField]
    private Text sizeLabel;

    private List<string> currentItems;

    public CargoHold CargoHold
    {
        get { return cargoHold; }
        set { cargoHold = value; }
    }

    public int HighlightedIndex
    {
        get
        {
            if (highlightedIndex >= cargoHold.Items.Count)
            {
                return -1;
            }

            return highlightedIndex;
        }
        set
        {
            highlightedIndex = value;
            UpdateHighlight();
        }
    }

    public ItemType HighlightedItem
    {
        get
        {
            var index = HighlightedIndex;
            if (index < 0)
            {
                return null;
            }

            var itemName = cargoHold.Items[index];
            return SpaceTraderConfig.CargoItemConfiguration.FindType(itemName);
        }
    }

    private void UpdateHighlight()
    {
        foreach (Transform child in itemsHolder.transform)
        {
            var item = child.GetComponent<CargoHoldListItem>();
            item.Highlighted = highlightedIndex == item.ItemIndex;
        }
    }

    public void Clear()
    {
        foreach (Transform child in itemsHolder.transform)
        {
            Destroy(child.gameObject);
        }

        currentItems = null;
        highlightedIndex = -1;
    }

    private void Update()
    {
        if (!CargoHold)
        {
            Clear();
            return;
        }

        if (sizeLabel)
        {
            sizeLabel.text = string.Format(sizeFormat, CargoHold.Items.Count, CargoHold.Size);
        }

        if (!CargoHold.Items.ElementsEquals(currentItems))
        {
            int oldHighlight = highlightedIndex;

            Clear();

            var itemCount = CargoHold.Items.Count;
            for (int itemIndex = 0; itemIndex < itemCount; ++itemIndex)
            {
                var item = CargoHoldListItem.CreateFromPrefab(listItem, CargoHold, itemIndex);

                item.transform.SetParent(itemsHolder.transform, false);
            }

            currentItems = new List<string>(CargoHold.Items);

            highlightedIndex = System.Math.Min(oldHighlight, currentItems.Count - 1);
        }

        UpdateHighlight();
    }

    private void OnSelectCargoItem(CargoHoldListItem item)
    {
        highlightedIndex = item.ItemIndex;
    }
}

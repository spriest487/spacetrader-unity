#pragma warning disable 0649

using UnityEngine;
using System.Linq;

public class CargoHoldList : MonoBehaviour
{
    [SerializeField]
    private CargoHold cargoHold;

    [SerializeField]
    private int highlightedIndex = CargoHold.BadIndex;

    //[SerializeField]
    //private string sizeFormat = "{0}/{1}";

    [Header("Prefabs")]

    [SerializeField]
    private CargoHoldListItem listItem;

    [Header("UI")]

    [SerializeField]
    private Transform itemsHolder;

    [SerializeField]
    private bool hideEmptySlots;

    [SerializeField]
    private bool allowHighlight = true;

    //[SerializeField]
    //private Text sizeLabel;

    private PooledList<CargoHoldListItem, ItemType> currentItems;

    public CargoHold CargoHold
    {
        get { return cargoHold; }
        set { cargoHold = value; }
    }

    public CargoHoldListItem this[int index]
    {
        get
        {
            Debug.Assert(currentItems != null);
            return currentItems[index];
        }
    }

    public int HighlightedIndex
    {
        get
        {
            if (!allowHighlight)
            {
                return CargoHold.BadIndex;
            }

            Debug.Assert(isActiveAndEnabled);

            if (!cargoHold.IsValidIndex(highlightedIndex))
            {
                return CargoHold.BadIndex;
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
            if (!allowHighlight)
            {
                return null;
            }

            var index = HighlightedIndex;
            if (index < 0)
            {
                return null;
            }

            return cargoHold[index];
        }
    }

    private void UpdateHighlight()
    {
        if (currentItems != null)
        {
            foreach (var item in currentItems)
            {
                item.Highlighted = allowHighlight && (highlightedIndex == item.ItemIndex);
            }
        }
    }

    private void Update()
    {
        Refresh();
        UpdateHighlight();
    }

    public void Refresh()
    {
        if (currentItems == null)
        {
            currentItems = new PooledList<CargoHoldListItem, ItemType>(itemsHolder, listItem);
        }

        if (cargoHold)
        {
            var items = CargoHold.Items;
            if (hideEmptySlots)
            {
                items = items.Where(i => !!i);
            }

            var newItems = currentItems.Refresh(items, (i, existingItem, cargoItem) =>
                existingItem.Assign(CargoHold, i));

            if (newItems)
            {
                BroadcastMessage("OnCargoListNewItems", currentItems.Items, SendMessageOptions.DontRequireReceiver);
            }

            highlightedIndex = System.Math.Min(highlightedIndex, CargoHold.Size - 1);
        }
        else
        {
            currentItems.Clear();

            highlightedIndex = CargoHold.BadIndex;
        }
    }

    private void OnSelectCargoItem(CargoHoldListItem item)
    {
        highlightedIndex = item.ItemIndex;
    }

    private void OnDragCargoItem(CargoHoldListItem item)
    {
        highlightedIndex = item.ItemIndex;

    }
}

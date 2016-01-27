using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CargoHoldList : MonoBehaviour
{
    [SerializeField]
    private CargoHold cargoHold;

    [SerializeField]
    private CargoHoldListItem listItem;

    [SerializeField]
    private Transform itemsHolder;

    private List<string> currentItems;

    public CargoHold CargoHold
    {
        get { return cargoHold; }
        set { cargoHold = value; }
    }

    private void Clear()
    {
        foreach (Transform child in itemsHolder.transform)
        {
            Destroy(child.gameObject);
        }

        currentItems = null;
    }

    private void Update()
    {
        if (!CargoHold)
        {
            Clear();
            return;
        }

        if (!CargoHold.Items.ElementsEquals(currentItems))
        {
            Clear();

            foreach (var cargoItem in CargoHold.Items)
            {
                var itemType = SpaceTraderConfig.CargoItemConfiguration.FindType(cargoItem);
                var item = CargoHoldListItem.CreateFromPrefab(listItem, itemType, 1);

                item.transform.SetParent(itemsHolder.transform, false);
            }

            currentItems = new List<string>(CargoHold.Items);
        }
    }
}

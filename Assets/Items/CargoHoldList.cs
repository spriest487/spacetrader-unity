using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(LayoutGroup))]
public class CargoHoldList : MonoBehaviour
{
    [SerializeField]
    private CargoHold cargoHold;

    [SerializeField]
    private CargoHoldListItem listItem;

    [SerializeField]
    private Transform itemsHolder;

    private List<string> currentItems;
        
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
        var targetHold = cargoHold ? cargoHold : PlayerShip.LocalPlayer.Ship.Cargo;

        if (targetHold)
        {
            var items = new List<string>(targetHold.Items);

            if (!items.ElementsEquals(currentItems))
            {
                Clear();

                foreach (var cargoItem in items)
                {
                    var itemType = SpaceTraderConfig.CargoItemConfiguration.FindType(cargoItem);
                    var item = CargoHoldListItem.CreateFromPrefab(listItem, itemType, 1);

                    item.transform.SetParent(itemsHolder.transform, false);
                }

                currentItems = items;
            }
        }
        else
        {
            Clear();
        }
    }

}

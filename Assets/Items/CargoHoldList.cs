using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LayoutGroup))]
public class CargoHoldList : MonoBehaviour
{
    [SerializeField]
    CargoHold cargoHold;

    [SerializeField]
    CargoHoldListItem listItem;
        
    void Update()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        var targetHold = cargoHold ? cargoHold : PlayerShip.LocalPlayer.GetComponent<CargoHold>();

        if (targetHold)
        {
            foreach (var cargoItem in targetHold.Items)
            {
                var itemType = SpaceTraderConfig.CargoItemConfiguration.FindType(cargoItem);
                var item = CargoHoldListItem.CreateFromPrefab(listItem, itemType, 1);
                item.transform.SetParent(transform, false);
            }
        }
    }

}

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
                var item = CargoHoldListItem.CreateFromPrefab(listItem, cargoItem, 1);
                item.transform.SetParent(transform, false);
            }
        }
    }

}

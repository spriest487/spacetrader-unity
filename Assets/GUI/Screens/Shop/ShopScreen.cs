using UnityEngine;
using System.Collections.Generic;

public class ShopScreen : MonoBehaviour
{
    [SerializeField]
    private CargoHoldList cargoItems;

    [SerializeField]
    private CargoHoldList forSaleItems;

    private void Update()
    {
        var player = SpaceTraderConfig.LocalPlayer;
        var station = player.GetComponent<Moorable>().SpaceStation;

        cargoItems.CargoHold = player.Ship.Cargo;
        forSaleItems.CargoHold = station.ItemsForSale;
    }
}

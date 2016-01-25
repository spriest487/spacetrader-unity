using UnityEngine;
using System;

[Serializable]
public class ShipForSale
{
    [SerializeField]
    private ShipType shipType;

    [SerializeField]
    private int price;

    public ShipType ShipType
    {
        get { return shipType; }
    }

    public int Price
    {
        get { return price; }
    }

    public ShipForSale()
    {
    }

    public ShipForSale(ShipType shipType, int price)
    {
        this.shipType = shipType;
        this.price = price;
    }
}
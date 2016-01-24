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
}
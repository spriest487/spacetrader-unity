using UnityEngine;
using System;

public class LootWindowItem : MonoBehaviour
{
    [SerializeField]
    private CargoHold cargoHold;

    [SerializeField]
    private int slot;

    public static LootWindowItem Create(LootWindowItem prefab, CargoHold cargo, int slot)
    {
        var item = Instantiate(prefab);
        item.Assign(cargo, slot);

        return item;
    }

    public void Assign(CargoHold cargo, int slot)
    {
        this.cargoHold = cargo;
        this.slot = slot;
    }

    public void Take()
    {
        var item = cargoHold[slot];
        var player = PlayerShip.LocalPlayer;
        
        if (player.Ship && player.Ship.Cargo.FreeCapacity > 0)
        {
            player.Ship.Cargo.Add(item);
            cargoHold.RemoveAt(slot);
            
            //TODO
            //cargoHold.Pack();
        }
        else
        {
            Debug.LogError("cargo error not handled");
        }
    }
}
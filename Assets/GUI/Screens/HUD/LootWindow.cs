#pragma warning disable 0649

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CargoHoldList))]
public class LootWindow : MonoBehaviour
{
    [SerializeField]
    private LootContainer loot;
   
    public LootContainer Container
    {
        get { return loot; }
        set { loot = value; }
    }
    
    private CargoHoldList cargoList;

    private void Start()
    {
        cargoList = GetComponent<CargoHoldList>();
    }
        
    private void Update()
    {
        if (!loot)
        {
            gameObject.SetActive(false);
        }
        else
        {
            cargoList.CargoHold = loot.Ship.Cargo;
        }
    }

    public void TakeAll()
    {
        var items = Container.Ship.Cargo;
        var activator = PlayerShip.LocalPlayer.Ship;

        var itemCount = items.ItemCount;
        var freeSpace = activator.Cargo.FreeCapacity;

        if (freeSpace >= itemCount)
        {
            for (int slot = 0; slot < items.Size; ++slot)
            {
                if (items.IsIndexFree(slot))
                {
                    continue;
                }

                var item = items[slot];
                activator.Cargo.Add(item);
                items.RemoveAt(slot);
            }

            Destroy(gameObject);
        }
    }
}

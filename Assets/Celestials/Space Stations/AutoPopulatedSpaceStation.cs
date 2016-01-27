using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(SpaceStation))]
public class AutoPopulatedSpaceStation : MonoBehaviour
{
    private SpaceStation station;

    public void Start()
    {
        station = GetComponent<SpaceStation>();
    }

    public void Populate()
    {
        var crewCount = UnityEngine.Random.Range(0, 5);
        var newCrew = new List<CrewMember>(crewCount);
        
        for (int crewNo = 0; crewNo < crewCount; ++crewNo)
        {
            var member = CrewMember.Create("No name");
            newCrew.Add(member);
        }

        station.AvailableCrew = newCrew;

        //for now, all ships are available everywhere
        var shipsForSale = new List<ShipForSale>();
        foreach (var shipType in SpaceTraderConfig.Market.BuyableShipTypes)
        {
            var price = SpaceTraderConfig.Market.GetShipPrice(shipType);
            var item = new ShipForSale(shipType, price);

            shipsForSale.Add(item);
        }
        station.ShipsForSale = shipsForSale;

        //all items available
        for (int slot = 0; slot < station.ItemsForSale.Size; ++slot)
        {
            //clear
            station.ItemsForSale.RemoveAt(slot);
        }

        foreach (var itemType in SpaceTraderConfig.CargoItemConfiguration.ItemTypes)
        {
            station.ItemsForSale.Add(itemType.name);
        }
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(AutoPopulatedSpaceStation))]
public class AutoPopulatedSpaceStationInspector : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var station = (AutoPopulatedSpaceStation)target;

        if (GUILayout.Button("Populate now"))
        {
            station.Populate();
        }
    }
}
#endif

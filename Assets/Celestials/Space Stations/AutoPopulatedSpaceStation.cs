using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(SpaceStation))]
public class AutoPopulatedSpaceStation : MonoBehaviour
{
    private SpaceStation station;

    public void Start()
    {
        station = GetComponent<SpaceStation>();

        Populate();
    }

    private void PopulateCrew()
    {
        foreach (var crew in station.FindAvailableCrew())
        {
            SpaceTraderConfig.CrewConfiguration.DestroyCharacter(crew);
        }

        var crewCount = UnityEngine.Random.Range(0, 5);
        var newCrew = new List<CrewMember>(crewCount);

        for (int crewNo = 0; crewNo < crewCount; ++crewNo)
        {
            //TODO: people have faces
            var member = SpaceTraderConfig.CrewConfiguration.NewCharacter("No name", null);
            member.RandomStats(3);
            member.Unassign(station);
            newCrew.Add(member);
        }

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
            station.ItemsForSale.Add(itemType);
        }
    }

    public void PopulateQuests()
    {
        var questBoard = SpaceTraderConfig.QuestBoard;

        var questsWithOwners = questBoard.QuestsAtStation(station)
            .Where(q => questBoard.OwnerOf(q))
            .ToList();

        questBoard.QuestsAtStation(station)
            .Except(questsWithOwners)
            .ToList()
            .ForEach(questBoard.CancelQuest);

        var targetQuestCount = UnityEngine.Random.Range(0, 5);
        var questTypes = questBoard.QuestTypes.ToList();
        for (int i = questsWithOwners.Count; i < targetQuestCount; ++i)
        {
            var randomType = UnityEngine.Random.Range(0, questTypes.Count);
            var newQuest = Quest.Create(questTypes[randomType], station);

            questBoard.NewQuest(newQuest);
        }
    }

    public void Populate()
    {
        PopulateCrew();
        PopulateQuests();
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

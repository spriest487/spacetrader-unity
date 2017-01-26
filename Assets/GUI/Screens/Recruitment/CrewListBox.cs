#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class CrewListBox : MonoBehaviour
{
    public enum TargetCrew
    {
        Player,
        Station
    }

    [SerializeField]
    private TargetCrew targetCrew;

    [SerializeField]
    private CrewAssignment forAssignment;

    [Header("Prefabs")]

    [SerializeField]
    private CrewListItem itemPrefab;

    [Header("UI")]

    [SerializeField]
    private Transform contentArea;

    [SerializeField]
    private Transform emptyLabel;

    private PooledList<CrewListItem, CrewMember> crewItems;

    private void OnEnable()
    {
        if (crewItems != null)
        {
            crewItems.Clear();
        }
        Update();
    }

    private void Update()
    {
        IEnumerable<CrewMember> crew = null;

        Ship playerShip = PlayerShip.LocalPlayer ? PlayerShip.LocalPlayer.Ship : null;

        if (playerShip)
        {
            if (targetCrew == TargetCrew.Station)
            {
                var station = PlayerShip.LocalPlayer.Dockable.DockedAtStation;
                if (station)
                {
                    crew = station.FindAvailableCrew();
                }
            }
            else
            {
                switch (forAssignment)
                {
                    case CrewAssignment.Captain:
                        crew = playerShip.GetCaptain().AsOptionalObject();
                        break;
                    default:
                        crew = playerShip.GetPassengers();
                        break;
                }
            }
        }

        if (crewItems == null)
        {
            crewItems = new PooledList<CrewListItem, CrewMember>(contentArea, itemPrefab);
        }

        if (crew != null && crew.Any())
        {
            CrewListItem.BuySellMode buySellMode;

            if (playerShip.Dockable.DockedAtStation)
            {
                if (targetCrew == TargetCrew.Player)
                {
                    buySellMode = CrewListItem.BuySellMode.Sellable;
                }
                else
                {
                    buySellMode = CrewListItem.BuySellMode.Buyable;
                }
            }
            else
            {
                buySellMode = CrewListItem.BuySellMode.ReadOnly;
            }

            crewItems.Refresh(crew, (i, existingItem, newCrewMember) =>
                existingItem.Assign(newCrewMember, buySellMode));

            emptyLabel.gameObject.SetActive(false);
        }
        else
        {
            crewItems.Clear();

            emptyLabel.gameObject.SetActive(true);
        }
    }
}
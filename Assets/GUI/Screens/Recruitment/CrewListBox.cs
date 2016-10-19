﻿#pragma warning disable 0649

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

    private void OnScreenActive()
    {
        if (crewItems != null)
        {
            crewItems.Clear();
        }
        Update();
    }
    
    private void Update()
    {
        IEnumerable<CrewMember> crew;

        Ship playerShip = PlayerShip.LocalPlayer ? PlayerShip.LocalPlayer.Ship : null;

        if (targetCrew == TargetCrew.Station)
        {
            var station = PlayerShip.LocalPlayer.Moorable.DockedAtStation;

            if (station)
            {
                crew = station.AvailableCrew;
            }
            else
            {
                crew = null;
            }
        }
        else
        {
            if (playerShip) {
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
            else
            {
                crew = null;
            }
        }

        if (crewItems == null)
        {
            crewItems = new PooledList<CrewListItem, CrewMember>(contentArea, itemPrefab);
        }

        if (crew != null && crew.Any())
        {
            CrewListItem.BuySellMode buySellMode;

            if (playerShip.Moorable.DockedAtStation)
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
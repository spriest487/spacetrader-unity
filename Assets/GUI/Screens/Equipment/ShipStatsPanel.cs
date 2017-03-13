#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShipStatsPanel : MonoBehaviour
{
    [SerializeField]
    private ShipStatsEntry statsEntryPrefab;

    [SerializeField]
    private Transform content;
    
    private PooledList<ShipStatsEntry, KeyValuePair<string, string>> items;
    
    private void OnEnable()
    {
        Refresh();
    }

    private void OnPlayerShipConfigChanged()
    {
        Refresh();
    }

    private void Refresh()
    {
        if (items == null)
        {
            items = new PooledList<ShipStatsEntry, KeyValuePair<string, string>>(content, statsEntryPrefab);
        }

        if (!Universe.LocalPlayer)
        {
            items.Clear();
            return;
        }

        var ship = Universe.LocalPlayer.Ship;
        var stats = ship.CurrentStats;
        var hp = ship.GetComponent<Hitpoints>();

        var entries = new Dictionary<string, string>
        {
            { "DPS", ship.EstimateDps().ToString("F2") },
            { "Max speed", stats.MaxSpeed.ToString("F2") + "m/s" },
            { "Agility", stats.MaxTurnSpeed.ToString("F2") + "deg/s" },
            { "Armor", hp.GetMaxArmor().ToString() },
            { "Shield", hp.GetMaxShields().ToString() },
            { "Mass", (stats.Mass * 0.001f).ToString("F2") + "t" }
        };

        if (items == null)
        {
            items = new PooledList<ShipStatsEntry, KeyValuePair<string, string>>(content, statsEntryPrefab);
        }

        items.Refresh(entries, (i, item, entry) => item.SetText(entry.Key, entry.Value));
    }
}
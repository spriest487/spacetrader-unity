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
    
    private void OnScreenActive()
    {
        Refresh();
    }

    private void OnPlayerShipConfigChanged()
    {
        Refresh();
    }

    private void Refresh()
    {
        var ship = PlayerShip.LocalPlayer.Ship;
        var stats = ship.CurrentStats;
        var hp = ship.GetComponent<Hitpoints>();

        var hardpoints = ship.ModuleLoadout;
      
        var entries = new Dictionary<string, string>();
        entries.Add("DPS", stats.estimatedDps.ToString("F2"));
        entries.Add("Max speed", stats.maxSpeed.ToString("F2") +"m/s");
        entries.Add("Agility", stats.maxTurnSpeed.ToString("F2") + "deg/s");
        entries.Add("Armor", hp.GetMaxArmor().ToString());
        entries.Add("Front Shield", hp.GetMaxShields(0).ToString());
        entries.Add("Rear Shield", hp.GetMaxShields(1).ToString());

        if (items == null)
        {
            items = new PooledList<ShipStatsEntry, KeyValuePair<string, string>>(content);
        }

        items.Refresh(entries,
            onNewItem: entry => ShipStatsEntry.Create(statsEntryPrefab, entry.Key, entry.Value),
            onUpdateItem: (item, entry) => item.SetText(entry.Key, entry.Value));
    }
}
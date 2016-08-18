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
      
        var entries = new Dictionary<string, string>();
        entries.Add("DPS", ship.EstimateDps().ToString("F2"));
        entries.Add("Max speed", stats.MaxSpeed.ToString("F2") +"m/s");
        entries.Add("Agility", stats.MaxTurnSpeed.ToString("F2") + "deg/s");
        entries.Add("Armor", hp.GetMaxArmor().ToString());
        entries.Add("Shield", hp.GetMaxShields().ToString());
        entries.Add("Mass", (stats.Mass * 0.001f).ToString("F2") + "t");

        if (items == null)
        {
            items = new PooledList<ShipStatsEntry, KeyValuePair<string, string>>(content, statsEntryPrefab);
        }

        items.Refresh(entries, (i, item, entry) => item.SetText(entry.Key, entry.Value));
    }
}
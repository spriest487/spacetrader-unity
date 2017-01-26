#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class BuySellShipList : MonoBehaviour
{
    [Header("UI")]

    [SerializeField]
    private Image thumbnail;

    [SerializeField]
    private Text nameLabel;

    [SerializeField]
    private Text priceLabel;

    //TODO
    //[SerializeField]
    //private Transform emptyLabel;

    private ShipForSale selected;
    private List<ShipForSale> availableShips;

    private void OnEnable()
    {
        var station = SpaceTraderConfig.LocalPlayer.Dockable.DockedAtStation;

        availableShips = station.ShipsForSale.ToList();

        if (availableShips.Count > 0)
        {
            selected = availableShips[0];
        }
        else
        {
            selected = null;

            nameLabel.gameObject.SetActive(false);
            priceLabel.gameObject.SetActive(false);
            thumbnail.gameObject.SetActive(false);
        }
    }

    private void MoveSelection(int amount)
    {
        var selectedIndex = Mathf.Max(0, availableShips.IndexOf(selected));
        var newIndex = (selectedIndex + amount) % availableShips.Count;

        selected = availableShips[newIndex];
    }

    public void NextShip()
    {
        MoveSelection(+1);
    }

    public void PreviousShip()
    {
        MoveSelection(-1);
    }
}
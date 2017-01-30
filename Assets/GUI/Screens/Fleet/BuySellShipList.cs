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
        var station = Universe.LocalPlayer.Dockable.DockedAtStation;

        availableShips = station.ShipsForSale.ToList();

        SetSelection(0);
    }

    private void SetSelection(int selectIndex)
    {
        if (selectIndex > 0 && selectIndex < availableShips.Count)
        {
            selected = availableShips[selectIndex];

            nameLabel.gameObject.SetActive(true);
            nameLabel.text = selected.ShipType.name;

            priceLabel.gameObject.SetActive(true);
            priceLabel.text = Market.FormatCurrency(selected.Price);

            thumbnail.gameObject.SetActive(true);
            thumbnail.sprite = selected.ShipType.Thumbnail;
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

        SetSelection(newIndex);
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
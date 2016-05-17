using UnityEngine;

public class SellToStationDropTarget : MonoBehaviour
{
    private void OnDropCargoItem(CargoHoldListItem item)
    {
        Debug.Log("dropped item to sell to station");

        var player = PlayerShip.LocalPlayer;
        var station = player.CurrentStation;

        SpaceTraderConfig.Market.SellItemToStation(player, item.ItemIndex);
    }
}

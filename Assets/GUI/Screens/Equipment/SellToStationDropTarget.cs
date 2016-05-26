#pragma warning disable 0649

using UnityEngine;

public class SellToStationDropTarget : MonoBehaviour
{
    [SerializeField]
    private CargoHoldListItem slot;

    private void OnDropCargoItem(CargoHoldListItem item)
    {
        Debug.Log("dropped item to sell to station");

        var player = PlayerShip.LocalPlayer;
        var station = player.CurrentStation;

        SpaceTraderConfig.Market.SellItemToStation(player, item.ItemIndex);
    }

    private void OnCargoListNewItem(CargoHoldListItem item)
    {
        if (!slot)
        {
            var slotDropTarget = item.gameObject.AddComponent<SellToStationDropTarget>();
            slotDropTarget.slot = item;
        }
    }
}

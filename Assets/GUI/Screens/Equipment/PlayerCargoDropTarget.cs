#pragma warning disable 0649

using UnityEngine;

public class PlayerCargoDropTarget : MonoBehaviour
{
    [SerializeField]
    private CargoHoldListItem slot;
    
    private CargoHoldList playerCargoList;

    private void Start()
    {
        playerCargoList = GetComponentInParent<CargoHoldList>();
    }

    private void OnDropCargoItem(CargoHoldListItem item)
    {
        var playerCargo = PlayerShip.LocalPlayer.Ship.Cargo;
        var sourceCargo = item.CargoHold;

        int targetIndex;
        if (!slot)
        {
            targetIndex = System.Math.Min(item.ItemIndex, playerCargo.FirstFreeIndex);
        }
        else
        {
            targetIndex = slot.ItemIndex;
        }

        Debug.LogFormat("drop on player cargo, target slot {0}", targetIndex);

        if (targetIndex >= 0)
        {
            if (sourceCargo == playerCargo)
            {
                playerCargo.Swap(item.ItemIndex, targetIndex);
            }
        }

        playerCargoList.Refresh();
        playerCargoList.HighlightedIndex = targetIndex;
    }
    
    private void OnCargoListNewItem(CargoHoldListItem item)
    {
        if (!slot)
        {
            var slotDropTarget = item.gameObject.AddComponent<PlayerCargoDropTarget>();
            slotDropTarget.slot = item;
        }
    }
}
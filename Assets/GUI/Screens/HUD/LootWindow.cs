#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(CargoHoldList))]
public class LootWindow : MonoBehaviour
{
    [SerializeField]
    private LootContainer loot;

    [SerializeField]
    private Text title;
   
    public LootContainer Container
    {
        get { return loot; }
    }
    
    private CargoHoldList cargoList;
            
    private void Update()
    {
        if (!loot)
        {
            gameObject.SetActive(false);
        }
        else
        {
            cargoList.CargoHold = loot.Ship.Cargo;
        }
    }

    public void Dismiss()
    {
        loot = null;
        gameObject.SetActive(false);
    }

    public void ShowLoot(LootContainer loot)
    {
        //this can actually get called before Start(), so make sure the ref is set here
        cargoList = GetComponent<CargoHoldList>();

        this.loot = loot;
        title.text = loot.name;
        gameObject.SetActive(true);
        cargoList.Refresh();
    }

    private IEnumerator TakeItemRoutine(int itemIndex)
    {
        var request = new MarketRequests.PlayerTakeLootRequest(PlayerShip.LocalPlayer, Container, itemIndex);

        SpaceTraderConfig.Market.PlayerTakeLoot(request);

        do
        {
            yield return null;
        }
        while (!request.Done);

        if (request.Error != null)
        {
            ScreenManager.Instance.BroadcastScreenMessage(PlayerStatus.Flight, ScreenID.None, "OnPlayerError", request.Error);
        }
    }

    public void TakeAll()
    {
        SpaceTraderConfig.Instance.StartCoroutine(TakeItemRoutine(-1));
    }

    public void OnSelectCargoItem(CargoHoldListItem selected)
    {
        cargoList.HighlightedIndex = CargoHold.BAD_INDEX;

        SpaceTraderConfig.Instance.StartCoroutine(TakeItemRoutine(selected.ItemIndex));
    }
}

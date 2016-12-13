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

    public GUIElement Element { get; private set; }

    public LootContainer Container
    {
        get { return loot; }
    }

    private CargoHoldList cargoList;

    private void Awake()
    {
        Element = GetComponent<GUIElement>();

        Element.OnTransitionedOut += () => loot = null;

        cargoList = GetComponent<CargoHoldList>();
    }

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

    public void ShowLoot(LootContainer loot)
    {
        this.loot = loot;
        title.text = loot.name;

        cargoList.CargoHold = loot.Ship.Cargo;
        cargoList.Refresh();

        Element.Activate(true);
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
            GUIController.Current.BroadcastMessage("OnPlayerError", request.Error);
        }
    }

    public void TakeAll()
    {
        SpaceTraderConfig.Instance.StartCoroutine(TakeItemRoutine(-1));
    }

    public void OnSelectCargoItem(CargoHoldListItem selected)
    {
        cargoList.HighlightedIndex = CargoHold.BadIndex;

        SpaceTraderConfig.Instance.StartCoroutine(TakeItemRoutine(selected.ItemIndex));
    }
}

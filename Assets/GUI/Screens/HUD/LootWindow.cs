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

        Element.OnTransitionedOut += () =>
        {
            loot = null;
            gameObject.SetActive(false);
        };

        cargoList = GetComponent<CargoHoldList>();
    }

    private void Update()
    {
        if (!loot)
        {
            Element.Activate(false);
        }
        else
        {
            cargoList.CargoHold = loot.Ship.Cargo;
        }
    }

    public void ShowLoot(LootContainer loot)
    {
        //do this first so Awake() is called
        gameObject.SetActive(true);

        this.loot = loot;
        title.text = loot.name.ToUpper();

        cargoList.CargoHold = loot.Ship.Cargo;
        cargoList.Refresh();

        Element.Activate(true);
    }

    private IEnumerator TakeItemRoutine(int itemIndex)
    {
        var request = new MarketRequests.PlayerTakeLootRequest(Universe.LocalPlayer, Container, itemIndex);

        Universe.Market.PlayerTakeLoot(request);

        do
        {
            yield return null;
        }
        while (!request.Done);

        if (request.Error != null)
        {
            PlayerNotifications.Error(request.Error);
        }
    }

    public void TakeAll()
    {
        Universe.Instance.StartCoroutine(TakeItemRoutine(-1));
    }

    public void OnSelectCargoItem(CargoHoldListItem selected)
    {
        cargoList.HighlightedIndex = CargoHold.BadIndex;

        Universe.Instance.StartCoroutine(TakeItemRoutine(selected.ItemIndex));
    }
}

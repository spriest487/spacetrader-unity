#pragma warning disable 0649

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField]
    private Transform content;

    [SerializeField]
    private Transform cinemaBars;

    [SerializeField]
    private SpeechBubble speechBubble;

    [Header("Player Info")]
    [SerializeField]
    private Transform[] playerShipInfo;

    [SerializeField]
    private Image playerPortrait;

    [SerializeField]
    private Text playerName;

    [SerializeField]
    private Text playerMoney;

    [Header("Overlays")]

    [SerializeField]
    private LootWindow lootWindow;

    [SerializeField]
    private ErrorMessage errorMessage;

    private BracketManager bracketManager;

    private void Start()
    {
        bracketManager = GetComponentInChildren<BracketManager>();
    }

    private void Update()
    {
        var player = PlayerShip.LocalPlayer;

        bool cinemaMode = ScreenManager.Instance.CurrentCutscenePage != null
            || player.Moorable.State == DockingState.AutoDocking;
        
        content.gameObject.SetActive(!cinemaMode);
        cinemaBars.gameObject.SetActive(cinemaMode);
        
        
        if (!cinemaMode)
        {
            bool playerInfoVisible = PlayerShip.LocalPlayer && PlayerShip.LocalPlayer.Ship;
            foreach (var shipInfoItem in playerShipInfo)
            {
                shipInfoItem.gameObject.SetActive(playerInfoVisible);
            }

            if (playerInfoVisible)
            {
                //TODO: slow
                var captain = PlayerShip.LocalPlayer.Ship.GetCaptain();
                if (captain)
                {
                    playerName.text = captain.name;
                    playerPortrait.sprite = captain.Portrait;
                }
                else
                {
                    playerName.text = "Player";
                    playerPortrait.sprite = SpaceTraderConfig.CrewConfiguration.DefaultPortrait;
                }

                var money = Market.FormatCurrency(PlayerShip.LocalPlayer.Money);
                playerMoney.text = money;
            }
        }
    }

    private void OnPlayerActivatedLoot(LootContainer loot)
    {
        if (lootWindow.Container == loot)
        {
            lootWindow.TakeAll();
        }
        else
        {
            lootWindow.ShowLoot(loot);
        }
    }

    private void OnScreenActive()
    {
        speechBubble.Dismiss();

        errorMessage.Reset();

        lootWindow.Dismiss();
    }

    private void OnRadioSpeech(PlayerRadioMessage message)
    {
        var bracket = bracketManager.FindBracket(message.Source.gameObject);

        if (bracket)
        {
            speechBubble.Show(message.Message, 3, bracket.transform);
        }
    }

    private void OnPlayerError(string message)
    {
        errorMessage.ShowError(message);
    }
}
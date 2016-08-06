#pragma warning disable 0649

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField]
    public Transform content;

    [SerializeField]
    private Transform[] playerShipInfo;

    [SerializeField]
    private SpeechBubble speechBubble;
    
    [SerializeField]
    private Text playerName;

    [SerializeField]
    private Text playerMoney;

    [SerializeField]
    private LootWindow lootWindow;

    private BracketManager bracketManager;

    private void Start()
    {
        bracketManager = GetComponentInChildren<BracketManager>();
    }

    private void Update()
    {
        bool visible = true;

        if (ScreenManager.Instance.CurrentCutscenePage != null)
        {
            visible = false;
        }

        content.gameObject.SetActive(visible);
        
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
            }
            else
            {
                playerName.text = "Player";
            }

            var money = Market.FormatCurrency(PlayerShip.LocalPlayer.Money);
            playerMoney.text = money;
        }
    }

    private void OnPlayerActivatedLoot(LootContainer loot)
    {
        lootWindow.gameObject.SetActive(true);
        lootWindow.Container = loot;
    }

    private void OnScreenActive()
    {
        speechBubble.Dismiss();
    }

    private void OnRadioSpeech(PlayerRadioMessage message)
    {
        var bracket = bracketManager.FindBracket(message.Source.gameObject);

        if (bracket)
        {
            speechBubble.Show(message.Message, 3, bracket.transform);
        }
    }
}
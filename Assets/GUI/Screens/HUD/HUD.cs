#pragma warning disable 0649

using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour
{
    [SerializeField]
    public Transform content;

    [SerializeField]
    private Transform[] playerShipInfo;

    [SerializeField]
    private SpeechBubble speechBubble;

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

        bool playerInfoVisible = PlayerShip.LocalPlayer ? PlayerShip.LocalPlayer.Ship : false;
        foreach (var shipInfoItem in playerShipInfo)
        {
            shipInfoItem.gameObject.SetActive(playerInfoVisible);
        }
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
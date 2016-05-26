#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class MoneyLabel : MonoBehaviour
{
    private Text text;

    private void Start()
    {
        text = GetComponent<Text>();
    }

    private void Update()
    {
        var player = PlayerShip.LocalPlayer;
        if (player)
        {
            text.text = Market.FormatCurrency(player.Money);
        }
        else
        {
            text.text = "-";
        }
    }
}
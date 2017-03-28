#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    private HandController hand;

    [SerializeField]
    private Text shipNameLabel;

    [SerializeField]
    private Text shipTypeLabel;

    [SerializeField]
    private Text newOrderLabel;

    private void Awake()
    {
        hand = GetComponentInParent<HandController>();
        Debug.Assert(hand, "InfoPanel must be a child of a HandController");
    }
    
    private void LateUpdate()
    {
        var focusShip = hand.Focus ?? hand.Hotspot.TouchingShip;

        if (focusShip)
        {
            shipNameLabel.text = focusShip.name;
            shipTypeLabel.text = focusShip.ShipType ? focusShip.ShipType.name : "";

            var orderLineGradient = hand.OrderLineColor(hand.PendingOrder);
            var orderLineColor = orderLineGradient != null ? orderLineGradient.Evaluate(1) : Color.white;
            
            newOrderLabel.text = hand.PendingOrder.ToString();
            newOrderLabel.color = orderLineColor;

            switch (hand.PendingOrder)
            {
                case AIOrder.Attack:
                    var target = focusShip;
                    newOrderLabel.text = "Attack " + target.name;
                    break;
                case AIOrder.Move:
                    newOrderLabel.text = "Move";
                    break;
                default:
                    newOrderLabel.text = "";
                    break;
            }
        }
        else
        {
            shipNameLabel.text = "";
            shipTypeLabel.text = "";
            newOrderLabel.text = "";
        }
    }
}

using UnityEngine;
using System;
using UnityEngine.UI;

[Serializable]
[RequireComponent(typeof(Button))]
public class CareerOptionButton : MonoBehaviour
{
    [SerializeField]
    private ShipType shipType;
    
    [SerializeField]
    private string description;

    public ShipType ShipType { get { return shipType; } }
    public string Description { get { return description; } }

    public void SetHighlight(bool highlighted)
    {
        GetComponent<Button>().interactable = !highlighted;
    }
}

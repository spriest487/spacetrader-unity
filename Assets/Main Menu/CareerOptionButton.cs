#pragma warning disable 0649

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

    [Header("Starting skills")]

    [SerializeField]
    private int pilotSkill;

    [SerializeField]
    private int weaponsSkill;

    [SerializeField]
    private int mechSkill;

    public ShipType ShipType { get { return shipType; } }
    public string Description { get { return description; } }
    public int PilotSkill { get { return pilotSkill; } }
    public int WeaponsSkill { get { return weaponsSkill; } }
    public int MechanicalSkill { get { return mechSkill; } }

    public void SetHighlight(bool highlighted)
    {
        GetComponent<Button>().interactable = !highlighted;
    }
}

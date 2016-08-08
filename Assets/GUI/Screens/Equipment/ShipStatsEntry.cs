#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

public class ShipStatsEntry : MonoBehaviour
{
    [SerializeField]
    private Text label;

    [SerializeField]
    private Text value;
    
    public void SetText(string label, string value)
    {
        this.label.text = label;
        this.value.text = value;
    }
}
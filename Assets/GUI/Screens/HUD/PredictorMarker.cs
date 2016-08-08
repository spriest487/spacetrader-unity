#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

public class PredictorMarker : MonoBehaviour
{
    [SerializeField]
    private Image image;

    public Color Color
    {
        get { return image.color; }
        set { image.color = value; }
    }
}

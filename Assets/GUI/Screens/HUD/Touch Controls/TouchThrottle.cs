using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class TouchThrottle : MonoBehaviour
{
    private static TouchThrottle instance;

    private Slider slider;

    public static float Value
    {
        get { return instance? instance.slider.value : 0; }
    }

    public static bool Available
    {
        get { return instance; }
    }

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        instance = this;
    }

    private void OnDisable()
    {
        instance = null;
    }
}
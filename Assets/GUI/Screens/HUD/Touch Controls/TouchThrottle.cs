using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Slider))]
public class TouchThrottle : MonoBehaviour
{
    private static TouchThrottle instance;

    private Slider slider;
    private bool dragging;

    public static float? Value
    {
        get
        {
            if (!instance || !instance.dragging)
            {
                return null;
            }

            return instance.GetValue();
        }
    }

    public static bool Available
    {
        get { return instance; }
    }

    [SerializeField]
    private float zeroPoint = 0.25f;

    private EventTrigger.Entry EventEntry(EventTriggerType eventID, Action callback)
    {
        var entry = new EventTrigger.Entry
        {
            eventID = eventID,
            callback = new EventTrigger.TriggerEvent()
        };

        entry.callback.AddListener(ev => callback());
        return entry;
    }

    private void Awake()
    {
        slider = GetComponent<Slider>();

    }

    private void Update()
    {
        var player = SpaceTraderConfig.LocalPlayer;
        if (!dragging && player && player.Ship)
        {
            var thrust = player.Ship.Thrust;
            if (thrust >= 0)
            {
                var range = 1 - zeroPoint;
                slider.value = (thrust / range) + zeroPoint;
            }
            else
            {
                var range = zeroPoint;
                slider.value = (1 + thrust) * zeroPoint;
            }
        }
    }

    private float GetValue()
    {
        var sliderPos = instance.slider.normalizedValue;

        float adjusted = sliderPos - zeroPoint;

        float positive = Mathf.Clamp(adjusted / (1 - zeroPoint), 0, 1);
        float negative = Mathf.Clamp(adjusted / zeroPoint, -1, 0);

        return positive + negative;
    }

    public void SetDragging(bool dragging)
    {
        this.dragging = dragging;
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
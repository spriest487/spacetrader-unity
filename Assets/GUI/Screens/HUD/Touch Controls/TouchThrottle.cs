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

    public static float Value
    {
        get
        {
            if (!instance)
            {
                return 0;
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

    [SerializeField]
    private float snapToZero = 0.1f;

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

    private void Start()
    {
        slider.value = zeroPoint;
    }
    
    private float ThrustToSliderVal(float thrust)
    {
        if (thrust >= 0)
        {
            var range = 1 - zeroPoint;
            return (thrust / range) + zeroPoint;
        }
        else
        {
            return (1 + thrust) * zeroPoint;
        }
    }

    private void Update()
    {
        var player = Universe.LocalPlayer;

        if (!(player && player.Ship))
        {
            return;
        }

        if (!dragging)
        {
            if (slider.value < zeroPoint)
            {
                slider.value = zeroPoint;
            }

            if (Mathf.Abs(slider.value - zeroPoint) < snapToZero)
            {
                slider.value = zeroPoint;
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
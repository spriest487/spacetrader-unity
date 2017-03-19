#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TouchJoystick : MonoBehaviour
{
    private static TouchJoystick instance;

    [SerializeField]
    private Image marker;

    [SerializeField]
    private float deadZone = 0.15f;

    private RectTransform rectTransform;

    private Vector2 value;

    private List<RaycastResult> raycastBuffer;

    public static Vector2 Value
    {
        get { return instance? instance.value : Vector2.zero; }
    }

    public static bool Available
    {
        get { return instance; }
    }

    public int? FingerID { get; private set; }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        instance = this;
    }

    private void OnDisable()
    {
        instance = null;
    }

    private bool BeginTouch(Touch touch, PointerEventData pointerEvent)
    {
        if (FingerID.HasValue)
        {
            return false;
        }

        pointerEvent.position = touch.position;

        if (raycastBuffer == null)
        {
            raycastBuffer = new List<RaycastResult>(2);
        }
        EventSystem.current.RaycastAll(pointerEvent, raycastBuffer);

        if (raycastBuffer.Count == 0)
        {
            return false;
        }

        var firstHit = raycastBuffer[0];
        if (firstHit.gameObject == this.gameObject)
        {
            Vector2 localPos;
            var hit = RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                touch.position,
                null,
                out localPos
            );

            if (hit && rectTransform.rect.Contains(localPos))
            {
                FingerID = touch.fingerId;

                return true;
            }
        }

        return false;
    }

    private float ApplyDeadzone(float val)
    {
        Debug.Assert(deadZone < 1 && deadZone >= 0);

        var range = 1 - deadZone;
        var power = Mathf.Abs(Mathf.Clamp(val - deadZone, -1, 1)) / range;

        return val * power;
    }

    private void UpdateActiveTouch(Touch touch)
    {
        if (!(FingerID.HasValue && touch.fingerId == FingerID.Value))
        {
            return;
        }

        Vector2 localPos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            touch.position,
            null,
            out localPos
        );

        var rect = rectTransform.rect;

        //apply pivot offset from center
        var pivot = rectTransform.pivot;
        var pivotOffset = new Vector2(rect.width * pivot.x, rect.height * pivot.y);
        var center = new Vector2(rect.width * 0.5f, rect.height * 0.5f);
        localPos += pivotOffset - center;



        Debug.Log(localPos.ToString("F3"));

        var inputLen = localPos.magnitude;
        if (Mathf.Approximately(inputLen, 0))
        {
            value = Vector2.zero;
            marker.rectTransform.anchoredPosition = Vector2.zero;
        }
        else
        {
            var maxDist = (rect.width * 0.5f) - (marker.rectTransform.rect.width * 0.5f);

            if (inputLen > maxDist)
            {
                localPos = (localPos / inputLen) * maxDist;
            }

            value = (localPos / maxDist);

            value = new Vector2(ApplyDeadzone(value.x), ApplyDeadzone(value.y));

            marker.rectTransform.anchoredPosition = localPos;
        }
    }

    private void FinishTouch(Touch touch)
    {
        if (!(FingerID.HasValue && touch.fingerId == FingerID.Value))
        {
            return;
        }

        FingerID = null;
        marker.rectTransform.anchoredPosition = Vector3.zero;
        value = Vector2.zero;
    }

    private void Update()
    {
        var touches = Input.touchCount;
        var pointerEvent = new PointerEventData(EventSystem.current);

        var e = Input.multiTouchEnabled;

        for (int touchIndex = 0; touchIndex < touches; ++touchIndex)
        {
            var touch = Input.GetTouch(touchIndex);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (BeginTouch(touch, pointerEvent))
                    {
                        goto case TouchPhase.Moved;
                    }
                    break;
                case TouchPhase.Stationary:
                case TouchPhase.Moved:
                    UpdateActiveTouch(touch);
                    break;
                case TouchPhase.Canceled:
                case TouchPhase.Ended:
                    FinishTouch(touch);
                    break;
            }
        }
    }
}
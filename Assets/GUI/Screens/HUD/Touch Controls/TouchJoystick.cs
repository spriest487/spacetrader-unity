using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

public class TouchJoystick : MonoBehaviour
{
    private static TouchJoystick instance;

    [SerializeField]
    private Image marker;

    [SerializeField]
    private Camera guiCamera;

    private RectTransform rectTransform;

    private Vector2 value;
    private int? fingerId;

    private List<RaycastResult> raycastBuffer;

    public static Vector2 Value
    {
        get { return instance? instance.value : Vector2.zero; }
    }

    public static bool Available
    {
        get { return instance; }
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        raycastBuffer = new List<RaycastResult>(2);
    }

    private void OnEnable()
    {
        instance = this;
    }

    private void OnDisable()
    {
        instance = null;
    }

    private void Update()
    {
        var touches = Input.touchCount;
        var pointerEvent = new PointerEventData(EventSystem.current);

        var e = Input.multiTouchEnabled;

        for (int touchIndex = 0; touchIndex < touches; ++touchIndex)
        {
            var touch = Input.GetTouch(touches);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (!fingerId.HasValue)
                    {
                        EventSystem.current.RaycastAll(pointerEvent, raycastBuffer);

                        var firstHit = raycastBuffer.First();
                        if (firstHit.gameObject == this)
                        {
                            Vector2 localPos;
                            var hit = RectTransformUtility.ScreenPointToLocalPointInRectangle(
                                rectTransform,
                                touch.position,
                                guiCamera,
                                out localPos
                            );

                            if (hit && rectTransform.rect.Contains(localPos))
                            {
                                fingerId = touch.fingerId;

                                goto case TouchPhase.Moved;
                            }


                        }
                    }
                    break;
                case TouchPhase.Stationary:
                case TouchPhase.Moved:
                    if (fingerId.HasValue && touch.fingerId == fingerId.Value)
                    {
                        Vector2 localPos;

                        RectTransformUtility.ScreenPointToLocalPointInRectangle(
                            rectTransform,
                            touch.position,
                            guiCamera,
                            out localPos
                        );

                        var inputLen = localPos.magnitude;
                        if (Mathf.Approximately(inputLen, 0))
                        {
                            value = Vector2.zero;
                            marker.rectTransform.anchoredPosition = Vector2.zero;
                        }
                        else
                        {
                            var maxDist = (rectTransform.rect.width * 0.5f) - (marker.rectTransform.rect.width * 0.5f);

                            if (inputLen > maxDist)
                            {
                                localPos = (localPos / inputLen) * maxDist;
                            }

                            value = localPos / maxDist;

                            marker.rectTransform.anchoredPosition = localPos;
                        }
                    }
                    break;
                case TouchPhase.Canceled:
                case TouchPhase.Ended:
                    if (fingerId.HasValue && touch.fingerId == fingerId.Value)
                    {
                        fingerId = null;
                        marker.rectTransform.anchoredPosition = Vector3.zero;
                        value = Vector2.zero;
                    }
                    break;
            }
        }
    }
}
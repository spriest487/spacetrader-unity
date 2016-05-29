#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

[RequireComponent(typeof(CanvasGroup))]
[ExecuteInEditMode]
public class SpeechBubble : MonoBehaviour
{
    [SerializeField]
    private Text text;

    [SerializeField]
    private Image icon;

    private CanvasGroup canvasGroup;
    private Coroutine currentShowRoutine;
    private Transform followTransform;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show(string message, float duration, Transform follow)
    {
        Dismiss();

        followTransform = follow;
        currentShowRoutine = StartCoroutine(ShowRoutine(message, duration));
    }

    public void Dismiss()
    {
        if (currentShowRoutine != null)
        {
            StopCoroutine(currentShowRoutine);
            currentShowRoutine = null;
            followTransform = null;
        }
    }

    private IEnumerator ShowRoutine(string message, float duration)
    {
        const float FADE_SPEED = 1 / 0.5f;

        text.text = message;
        canvasGroup.alpha = 0;
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += FADE_SPEED * Time.deltaTime;
            yield return null;
        }

        var endTime = Time.time + duration;
        do
        {
            yield return null;
        }
        while (Time.time < endTime);

        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= FADE_SPEED * Time.deltaTime;
            yield return null;
        }

        Dismiss();
    }

    private void Update()
    {
        if (followTransform)
        {
            transform.position = followTransform.position;
        }

        var pos = transform.position;
        float scaleX;
        float scaleY;
        
        if (pos.x > Screen.width / 2)
        {
            scaleX = -1;
        }
        else
        {
            scaleX = 1;
        }

        if (pos.y > Screen.height / 2)
        {
            scaleY = -1;
        }
        else
        {
            scaleY = 1;
        }

        var scale = new Vector3(scaleX, scaleY, 1);
        transform.localScale = scale;
        text.transform.localScale = scale;
        icon.transform.localScale = scale;
    }
}

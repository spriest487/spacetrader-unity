#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ErrorMessage : MonoBehaviour
{
    private Coroutine currentError;

    [SerializeField]
    private CanvasGroup group;

    public void ShowError(string message)
    {
        Reset();
        currentError = StartCoroutine(ErrorFade(message));
    }

    public void Reset()
    {
        if (currentError != null)
        {
            StopCoroutine(currentError);
        }

        group.alpha = 0;
    }

    private IEnumerator ErrorFade(string message)
    {
        group.GetComponentInChildren<Text>().text = message;
        group.alpha = 1;

        yield return new WaitForSeconds(2);

        while (group.alpha > 0)
        {
            group.alpha -= 1 * Time.deltaTime;
            yield return null;
        }
    }
}
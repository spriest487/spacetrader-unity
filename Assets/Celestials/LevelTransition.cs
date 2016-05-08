using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class LevelTransition : MonoBehaviour
{
    private static LevelTransition activeTransition = null;

    private Coroutine changingLevel = null;

    [SerializeField]
    private string level = null;
    
    private void OnActivated(Ship activator)
    {
        if (activeTransition)
        {
            if (activeTransition == this)
            {
                ScreenManager.Instance.BroadcastScreenMessage(PlayerStatus.Flight, ScreenID.None, "OnPlayerNotification", "Jump cancelled");

                StopCoroutine(changingLevel);
                activeTransition = null;
            }
            else
            {
                Debug.LogWarning("can't activate a level transition, one is already running");
            }
        }
        else
        {
            activeTransition = this;
            changingLevel = StartCoroutine(ChangeLevel());
        }
    }
    
    private IEnumerator ChangeLevel()
    {
        var tickWait = new WaitForSeconds(1);
        var messageTemplate = "Jumping to " +level +" in {0}...";

        for (int i = 3; i >= 0; --i)
        {
            if (i > 0)
            {
                var message = string.Format(messageTemplate, i);

                ScreenManager.Instance.BroadcastScreenMessage(PlayerStatus.Flight, ScreenID.None, "OnPlayerNotification", message);
            }

            yield return tickWait;
        }

        yield return tickWait;
        SceneManager.LoadScene(level);
    }
}
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public string newGameScene;
    public string worldCommonScene; 
    
    public string menuScene;

    public void NewGame()
    {
        Application.LoadLevel(newGameScene);
        Application.LoadLevelAdditive(worldCommonScene);
    }

    public void EndGame()
    {
        var worldLifecycleListeners = GameObject.FindGameObjectsWithTag("WorldListener");
        if (worldLifecycleListeners != null)
        {
            foreach (var worldListener in worldLifecycleListeners)
            {
                worldListener.SendMessage("OnWorldEnd");
            }
        }

        Application.LoadLevel(menuScene);
    }

    public void BackToGame()
    {
        if (ScreenManager.Instance)
        {
            ScreenManager.Instance.HudOverlay = ScreenManager.HudOverlayState.None;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    void Update()
    {
        var ingame = ScreenManager.Instance != null;

        foreach (var obj in GameObject.FindGameObjectsWithTag("UIOnlyIngame"))
        {
            obj.SetActive(ingame);
        }

        foreach (var obj in GameObject.FindGameObjectsWithTag("UIOnlyOutOfGame"))
        {
            obj.SetActive(!ingame);
        }
    }
}

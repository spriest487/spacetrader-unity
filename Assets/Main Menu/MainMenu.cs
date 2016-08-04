﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [System.Serializable]
    public class Screens
    {
        public Transform newGameScreen;
        public Transform rootScreen;
        public Transform missionsScreen;
    }

    [SerializeField]
    private Screens screens = new Screens();
            
    [SerializeField]
    private string menuScene;

    [SerializeField]
    private Transform[] activeWhenPlayerExists;

    [SerializeField]
    private Transform[] activeWhenNoPlayerExists;
    
    public void GoToRoot()
    {
        GoToScreen(null);
    }

    public void GoToScreen(string screenId)
    {
        var allScreens = new Transform[] {
            screens.newGameScreen,
            screens.rootScreen,
            screens.missionsScreen
        };
        
        Transform showScreen;

        switch (screenId)
        {
            case "newgame":
                {
                    showScreen = screens.newGameScreen;
                    break;
                }
            case "missions":
                {
                    showScreen = screens.missionsScreen;
                    break;
                }
            default:
                {
                    showScreen = screens.rootScreen;
                    break;
                }
        }

        foreach (var screen in allScreens)
        {
            var screenActive = screen == showScreen;
           
            if (!screenActive)
            {
                screen.SendMessage("OnMenuScreenDeactivate", SendMessageOptions.DontRequireReceiver);
            }

            screen.gameObject.SetActive(screenActive);
        }

        showScreen.SendMessage("OnMenuScreenActivate", SendMessageOptions.DontRequireReceiver);
    }

    public void NewGame()
    {
        GoToScreen("newgame");
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

        SceneManager.LoadScene(menuScene);
    }

    public void SaveGame()
    {
        SavedGames.SavesFolder.SaveGame();
    }

    public void LoadGame()
    {
        SavedGames.SavesFolder.LoadGame();
    }

    public void BackToGame()
    {
        ScreenManager.Instance.ScreenID = ScreenID.None;
    }
    
    public void Quit()
    {
        Application.Quit();
    }

    void Update()
    {
        var ingame = !!PlayerShip.LocalPlayer;

        foreach (var obj in activeWhenPlayerExists)
        {
            obj.gameObject.SetActive(ingame);
        }

        foreach (var obj in activeWhenNoPlayerExists)
        {
            obj.gameObject.SetActive(!ingame);
        }
    }

    void Start()
    {
        GoToRoot();
    }
}

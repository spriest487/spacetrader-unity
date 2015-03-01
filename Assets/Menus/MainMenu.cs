using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [System.Serializable]
    public class Screens
    {
        [SerializeField]
        public Transform rootScreen;

        [SerializeField]
        public Transform missionsScreen;
    }

    [SerializeField]
    private Text screenTitle;

    [SerializeField]
    private Screens screens = new Screens();

    [SerializeField]
    private string newGameScene;
        
    [SerializeField]
    private string menuScene;

    public void GoToRoot()
    {
        GoToScreen(null);
    }

    public void GoToScreen(string screenId)
    {
        var allScreens = new Transform[] {
            screens.rootScreen,
            screens.missionsScreen
        };
        
        Transform showScreen;

        switch (screenId)
        {
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

        if (screenTitle) 
        {
            if (showScreen == screens.rootScreen)
            {
                screenTitle.text = "";
            }
            else
            {
                screenTitle.text = showScreen.name;
            }
        }
    }

    public void NewGame()
    {
        Application.LoadLevel(newGameScene);
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

    void Start()
    {
        GoToRoot();
    }
}

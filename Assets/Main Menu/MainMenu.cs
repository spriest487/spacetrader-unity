using System.Collections.Generic;
using System.Collections;
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

    public void OnScreenActive()
    {
        GoToRoot();
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
        SpaceTraderConfig.Instance.StartCoroutine(EndGameRoutine());
    }

    private IEnumerator EndGameRoutine()
    {
        var worldLifecycleListeners = GameObject.FindGameObjectsWithTag("WorldListener");
        if (worldLifecycleListeners != null)
        {
            foreach (var worldListener in worldLifecycleListeners)
            {
                worldListener.SendMessage("OnWorldEnd");
            }
        }

        var loading = ScreenManager.Instance.CreateLoadingScreen();

        if (PlayerShip.LocalPlayer)
        {
            Destroy(PlayerShip.LocalPlayer.gameObject);
        }

        yield return SceneManager.LoadSceneAsync(menuScene);
        yield return new WaitForEndOfFrame();
        GoToRoot();

        loading.Dismiss();
    }

    public void SaveGame()
    {
        SavedGames.SavesFolder.SaveGame();

        if (PlayerShip.LocalPlayer)
        {
            ScreenManager.Instance.ScreenID = ScreenID.None;
            ScreenManager.Instance.BroadcastScreenMessage(PlayerStatus.Flight, ScreenID.None, "OnPlayerNotification", "Game saved");
        }
    }

    public void LoadGame()
    {
        SpaceTraderConfig.Instance.StartCoroutine(LoadGameRoutine());
    }

    private IEnumerator LoadGameRoutine()
    {
        var loading = ScreenManager.Instance.CreateLoadingScreen();

        var loadSave = SavedGames.SavesFolder.LoadGame();
        yield return loadSave;
        
        loading.Dismiss();

        if (loadSave.Error != null)
        {
            Debug.LogException(loadSave.Error);
        }
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

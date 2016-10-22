#pragma warning disable 0649

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(GUIScreen))]
public class MainMenu : MonoBehaviour
{           
    [SerializeField]
    private string menuScene;

    [SerializeField]
    private Transform[] activeWhenPlayerExists;

    [SerializeField]
    private Transform[] activeWhenNoPlayerExists;

    private GUIController guiController;

    private void Start()
    {
        guiController = GetComponentInParent<GUIController>();
    }
    
    public void NewGame()
    {
        guiController.SwitchTo(ScreenID.NewGame);
    }

    public void EndGame()
    {
        SpaceTraderConfig.Instance.StartCoroutine(EndGameRoutine());
    }

    public void LoadGame()
    {
        guiController.SwitchTo(ScreenID.LoadGame);
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

        guiController.SwitchTo(ScreenID.MainMenu);

        loading.Dismiss();
    }

    public void SaveGame()
    {
        SavedGames.SavesFolder.SaveGame();

        if (PlayerShip.LocalPlayer)
        {
            ScreenManager.Instance.ScreenID = ScreenID.None;
            ScreenManager.Instance.BroadcastScreenMessage(ScreenID.None, "OnPlayerNotification", "Game saved");
        }
    }

    public void BackToGame()
    {
        ScreenManager.Instance.TryFadeScreenTransition(ScreenID.None);
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
}

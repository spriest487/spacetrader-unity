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
    private GUIScreen guiScreen;
    
    private void OnEnable()
    {
        guiController = GetComponentInParent<GUIController>();
        guiScreen = GetComponent<GUIScreen>();

        var inGame = !!SpaceTraderConfig.LocalPlayer;

        guiScreen.ShowHeader = inGame;
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

        yield return guiController.ShowLoadingOverlay();

        if (PlayerShip.LocalPlayer)
        {
            Destroy(PlayerShip.LocalPlayer.gameObject);
        }

        yield return SceneManager.LoadSceneAsync(menuScene);
        yield return new WaitForEndOfFrame();

        guiController.DismissLoadingOverlay();
        guiController.SwitchTo(ScreenID.None);
    }

    private IEnumerator SaveGameRoutine()
    {
        SavedGames.SavesFolder.SaveGame();

        if (PlayerShip.LocalPlayer)
        {
            yield return guiController.SwitchTo(ScreenID.None);

            PlayerNotifications.GameMessage("Game Saved");
        }
    }

    public void SaveGame()
    {
        StartCoroutine(SaveGameRoutine());
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

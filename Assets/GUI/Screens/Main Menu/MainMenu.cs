#pragma warning disable 0649

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(GUIScreen))]
public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private Transform headerImage;

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
        headerImage.gameObject.SetActive(!inGame);
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

    public void MissionSelect()
    {
        guiController.SwitchTo(ScreenID.MissionSelect);
    }

    private IEnumerator EndGameRoutine()
    {
        yield return guiController.ShowLoadingOverlay();

        if (PlayerShip.LocalPlayer)
        {
            Destroy(PlayerShip.LocalPlayer.gameObject);
        }

        if (MissionManager.Instance.Mission)
        {
            yield return MissionManager.Instance.CancelMission();
        }
        else
        {
            yield return SpaceTraderConfig.WorldMap.LoadArea(null);
        }

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

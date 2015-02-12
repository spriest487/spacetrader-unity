using System.Collections;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public enum IngameState
    {
        Docked,
        Flight,
    }

    public bool menuState { get; private set; }
    private GameObject mainMenuInstance;

    public GameObject[] dockedScreen;
    public GameObject[] flightScreen;
    public GameObject mainMenu;

    private IngameState _ingameState = IngameState.Flight;
    public IngameState ingameState
    {
        get
        {
            return _ingameState;
        }
        set
        {
            _ingameState = value;
            Apply();
        }
    }

    public void Apply()
    {
        foreach (var dockedObj in dockedScreen)
        {
            dockedObj.SetActive(ingameState == IngameState.Docked);
        }

        foreach (var flightObj in flightScreen)
        {
            flightObj.SetActive(ingameState == IngameState.Flight);
        }

        mainMenu.SetActive(menuState);
    }

    void Start()
    {       
        mainMenuInstance = GameObject.Find("MainMenu");
        if (mainMenuInstance == null)
        {
            mainMenuInstance = (GameObject) Instantiate(mainMenu);
            mainMenuInstance.name = "MainMenu";
        }

        Apply();
    }
}
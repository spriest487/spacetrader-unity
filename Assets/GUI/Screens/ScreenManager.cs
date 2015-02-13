using System.Collections;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public enum ScreenState
    {
        Docked,
        Flight,
    }

    public static ScreenManager Instance { get; private set; }

    private GameObject mainMenuInstance;

    public GameObject[] dockedScreen;
    public GameObject[] flightScreen;
    public GameObject mainMenu;

    [SerializeField]
    private ScreenState state;

    public ScreenState State
    {
        get
        {
            return state;
        }
        private set
        {
            if (state != value)
            {
                state = value;
                Apply();
            }
        }
    }

    [SerializeField]
    private bool menuState;

    public bool MenuState
    {
        get
        {
            return menuState;
        }
        set
        {
            if (value != menuState)
            {
                menuState = value;
                Apply();
            }
        }
    }

    public void Apply()
    {
        foreach (var dockedObj in dockedScreen)
        {
            dockedObj.SetActive(state == ScreenState.Docked);
        }

        foreach (var flightObj in flightScreen)
        {
            flightObj.SetActive(state == ScreenState.Flight);
        }

        mainMenu.SetActive(MenuState);
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

    void Update()
    {
        bool docked = false;

        var player = PlayerStart.ActivePlayer;
        if (player)
        {
            var moorable = player.GetComponent<Moorable>();
            if (moorable && moorable.Moored)
            {
                docked = true;
            }
        }

        State = docked? ScreenState.Docked : ScreenState.Flight;
    }

    void OnEnable()
    {
        if (Instance != null)
        {
            throw new UnityException("screen manager already active");
        }

        Instance = this;
    }

    void OnDisable()
    {
        if (Instance != this)
        {
            throw new UnityException("inactive screen manager being disabled");
        }

        Instance = null;
    }
}
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenManager : MonoBehaviour
{
    private static ScreenManager instance;
    public static ScreenManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ScreenManager>();
            }

            return instance;
        }
        private set
        {
            instance = value;
        }
    }

    [Serializable]
    public class ScreenMapping
    {
        [SerializeField]
        private ScreenID screenId;

        [SerializeField]
        private PlayerStatus playerStatus;
        
        [SerializeField]
        private GameObject overlay;

        [HideInInspector]
        [SerializeField]
        private GameObject overlayInstance;
        
        public GameObject Overlay { get { return overlayInstance; } }

        public ScreenID ScreenID { get { return screenId; } }
        public PlayerStatus PlayerStatus { get { return playerStatus; } }
        
        public void Init()
        {
            if (!overlayInstance)
            {
                overlayInstance = Instantiate(overlay);
                DontDestroyOnLoad(overlayInstance.gameObject);
            }
        }
    }
    
    [SerializeField]
    private PlayerStatus playerStatus;
    
    [SerializeField]
    private ScreenID screenId;

    [SerializeField]
    private ScreenMapping[] screens = new ScreenMapping[0];

    [SerializeField]
    private ScreenBar screenBarPrefab;

    [HideInInspector]
    [SerializeField]
    private ScreenBar screenBar;

    [SerializeField]
    private bool menuState;

    [SerializeField]
    private Cutscene cutscene;

    public PlayerStatus State
    {
        get
        {
            return playerStatus;
        }
        private set
        {
            if (playerStatus != value)
            {
                playerStatus = value;
                Apply();
            }
        }
    }

    public ScreenID HudOverlay
    {
        get
        {
            return screenId;
        }
        set
        {
            screenId = value;
            Apply();
        }
    }    

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

    public CutscenePage CurrentCutscenePage
    {
        get
        {
            return cutscene ? cutscene.CurrentPage : null;
        }
    }

    public CutsceneCameraRig CurrentCutsceneCameraRig
    {
        get
        {
            return cutscene ? cutscene.CameraRig : null;
        }
    }

    private ScreenID DefaultHudOverlay
    {
        get
        {
            /*if there's an active mission, and the player is not spawned, the default state 
             is the mission prep screen instead */
            if (!!MissionManager.Instance && !PlayerShip.LocalPlayer)
            {
                return ScreenID.MissionPrep;
            }
            else if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                return ScreenID.MainMenu;
            }
            else
            {
                return ScreenID.None;
            }
        }
    }

    private void OnEnable()
    {
        if (Instance != null && instance != this)
        {
            //can't have two in a scene, and the existing one takes priority
            Destroy(gameObject);
            gameObject.SetActive(false);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private void OnDisable()
    {
        Instance = null;
    }

    public void BroadcastScreenMessage(PlayerStatus screen,
        ScreenID overlayState,
        string message, 
        object value)
    {
        foreach (var overlay in screens)
        {
            if (overlay.PlayerStatus == screen && overlay.ScreenID == overlayState)
            {
                overlay.Overlay.BroadcastMessage(message, value, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
    
    private void Apply()
    {
        if (!screenBar)
        {
            screenBar = Instantiate(screenBarPrefab);
        }

        foreach (var overlay in screens)
        {
            overlay.Init();

            var overlayActive = HudOverlay == overlay.ScreenID;
            var screenActive = State == overlay.PlayerStatus
                || overlay.PlayerStatus == PlayerStatus.None;

            var overlayState = overlayActive && screenActive;

            overlay.Overlay.gameObject.SetActive(overlayState);
            
            overlay.Overlay.BroadcastMessage(overlayState ? "OnScreenActive" : "OnScreenInactive", SendMessageOptions.DontRequireReceiver);
        }
    }

    public void ToggleOverlay(ScreenID state)
    {
        if (HudOverlay == state)
        {
            HudOverlay = DefaultHudOverlay;
        }
        else
        {
            HudOverlay = state;
        }

        Apply();
    }

    public void SetStates(ScreenID hudOverlay, PlayerStatus state)
    {
        this.screenId = hudOverlay;
        this.playerStatus = state;
        Apply();
    }

    public void PlayCutscene(Cutscene cutsceneToPlay)
    {
        Debug.Assert(cutsceneToPlay != null);

        cutscene = Instantiate(cutsceneToPlay);
        cutscene.Start();
    }

    public void AdvanceCutscene()
    {
        Debug.Assert(cutscene != null);

        cutscene.Next();

        if (cutscene.CurrentPage == null)
        {
            cutscene = null;
        }
    }

    private void Start()
    {
        HudOverlay = DefaultHudOverlay;
    }

    private void Update()
    {
        bool docked = false;

        var player = PlayerShip.LocalPlayer;
        if (player)
        {
            var moorable = player.GetComponent<Moorable>();
            if (moorable && moorable.Moored)
            {
                docked = true;
            }
        }

        State = docked? PlayerStatus.Docked : PlayerStatus.Flight;
    }

    private void OnLevelWasLoaded(int level)
    {
        Start();
    }
}
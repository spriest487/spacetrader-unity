using System.Collections;
using System;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    [Serializable]
    public class HudOverlayMapping
    {
        [SerializeField]
        private HudOverlayState state;

        [SerializeField]
        private ScreenState screenState;
        
        [SerializeField]
        private GameObject overlay;

        [SerializeField]
        private bool screenBarVisible;

        [HideInInspector]
        [SerializeField]
        private GameObject overlayInstance;

        [HideInInspector]
        [SerializeField]
        private ScreenBar barInstance;

        public GameObject Overlay { get { return overlayInstance; } }
        public ScreenBar Bar { get { return barInstance; } }

        public HudOverlayState State { get { return state; } }
        public ScreenState ScreenState { get { return screenState; } }

        public bool ScreenBarVisible { get { return screenBarVisible; } }

        public void Init()
        {
            if (!overlayInstance)
            {
                overlayInstance = Instantiate(overlay);
            }

            if (!barInstance && screenBarVisible)
            {
                //add the screenbar
                barInstance = Instantiate(Instance.screenBar);
                barInstance.transform.position = Vector3.zero;
                barInstance.transform.SetParent(overlayInstance.transform, false);
            }
        }
    }

    public enum ScreenState
    {
        None,
        Docked,
        Flight,
    }

    public enum HudOverlayState
    {
        None,
        MainMenu,
        Equipment,
        MissionPrep,
        Recruitment
    }

    public enum MissionPhase
    {
        Briefing,
        Active,
        Finished
    }

    [SerializeField]
    private ScreenState state;

    [SerializeField]
    private HudOverlayState hudOverlay;

    [SerializeField]
    private HudOverlayMapping[] hudOverlays;

    [SerializeField]
    private ScreenBar screenBar;

    [SerializeField]
    private bool menuState;

    public static ScreenManager Instance { get; private set; }

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

    public HudOverlayState HudOverlay
    {
        get
        {
            return hudOverlay;
        }
        set
        {
            hudOverlay = value;
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

    private void Apply()
    {
        foreach (var overlay in hudOverlays)
        {
            overlay.Init();

            var overlayActive = HudOverlay == overlay.State;
            var screenActive = State == overlay.ScreenState
                || overlay.ScreenState == ScreenState.None;

            var overlayState = overlayActive && screenActive;

            overlay.Overlay.gameObject.SetActive(overlayState);
            
            overlay.Overlay.BroadcastMessage(overlayState ? "OnScreenActive" : "OnScreenInactive", SendMessageOptions.DontRequireReceiver);
        }
    }

    public void ToggleOverlay(HudOverlayState state)
    {
        if (HudOverlay == state)
        {
            /*if there's an active mission, and the player is not spawned, the default state 
             is the mission prep screen instead */
            if (MissionManager.Instance != null && !PlayerShip.LocalPlayer)
            {
                HudOverlay = HudOverlayState.MissionPrep;
            }
            else
            {
                HudOverlay = HudOverlayState.None;
            }
        }
        else
        {
            HudOverlay = state;
        }

        Apply();
    }

    public void SetStates(HudOverlayState hudState, ScreenState state)
    {
        this.hudOverlay = hudState;
        this.state = state;
        Apply();
    }

    void Start()
    {      
        Apply();

        var missionManager = MissionManager.Instance;
        if (missionManager)
        {
            this.hudOverlay = HudOverlayState.MissionPrep;
        }
        else
        {
            this.hudOverlay = HudOverlayState.None;
        }
    }

    void Update()
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
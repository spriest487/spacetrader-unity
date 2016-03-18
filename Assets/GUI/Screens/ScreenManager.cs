using System.Collections;
using System;
using UnityEngine;

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
    private class HudOverlayMapping
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

        public void Init(ScreenBar screenBarPrefab)
        {
            if (!overlayInstance)
            {
                overlayInstance = Instantiate(overlay);
                DontDestroyOnLoad(overlayInstance.gameObject);
            }

            if (!barInstance && screenBarVisible)
            {
                //add the screenbar
                barInstance = Instantiate(screenBarPrefab);
                barInstance.transform.position = Vector3.zero;
                barInstance.transform.SetParent(overlayInstance.transform, false);
            }
        }
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

    [SerializeField]
    private Cutscene cutscene;

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

    public void BroadcastScreenMessage(ScreenState screen,
        HudOverlayState overlayState,
        string message, 
        object value)
    {
        foreach (var overlay in hudOverlays)
        {
            if (overlay.ScreenState == screen && overlay.State == overlayState)
            {
                overlay.Overlay.BroadcastMessage(message, value, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
    
    private void Apply()
    {
        foreach (var overlay in hudOverlays)
        {
            overlay.Init(screenBar);

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
            if (!!MissionManager.Instance && !PlayerShip.LocalPlayer)
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

    public void SetStates(HudOverlayState hudOverlay, ScreenState state)
    {
        this.hudOverlay = hudOverlay;
        this.state = state;
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
        Apply();
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

        State = docked? ScreenState.Docked : ScreenState.Flight;
    }

    private void OnLevelWasLoaded(int level)
    {
        if (FindObjectOfType<MissionManager>() != null)
        {
            //starting a mission
            HudOverlay = HudOverlayState.MissionPrep;
        }
        else if (level != 0) //0 is root menu
        {
            //loading any other scene
            HudOverlay = HudOverlayState.None;
        }
    }
}
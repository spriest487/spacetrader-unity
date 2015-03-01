using System.Collections;
using System;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance { get; private set; }

    [Serializable]
    public class HudOverlayMapping
    {
        [SerializeField]
        private HudOverlayState state;
        
        [SerializeField]
        private GameObject overlay;

        [HideInInspector]
        private GameObject overlayInstance;

        public GameObject Overlay
        {
            get
            {
                if (!overlayInstance)
                {
                    overlayInstance = (GameObject) Instantiate(overlay);
                }

                return overlayInstance;
            }
        }

        public HudOverlayState State { get { return state; } }
    }

    public enum ScreenState
    {
        Docked,
        Flight,
    }

    public enum HudOverlayState
    {
        None,
        Docked,
        MainMenu,
        Equipment
    }    

    [SerializeField]
    private ScreenState state;

    [SerializeField]
    private HudOverlayState hudOverlay;

    [SerializeField]
    private HudOverlayMapping[] hudOverlays;

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

    private void Apply()
    {
        foreach (var overlay in hudOverlays)
        {
            overlay.Overlay.SetActive(HudOverlay == overlay.State);
        }
    }

    public void ToggleOverlay(HudOverlayState state)
    {
        if (HudOverlay == state)
        {
            HudOverlay = HudOverlayState.None;
        }
        else
        {
            HudOverlay = state;
        }

        Apply();
    }

    void Start()
    {      
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
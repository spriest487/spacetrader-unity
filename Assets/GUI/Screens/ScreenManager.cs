﻿using System.Collections;
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
        private ScreenState screenState;
        
        [SerializeField]
        private GameObject overlay;

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

        public void Init()
        {
            if (!overlayInstance)
            {
                overlayInstance = (GameObject)Instantiate(overlay);
            }

            if (!barInstance)
            {
                //add the screenbar
                barInstance = (ScreenBar)Instantiate(ScreenManager.Instance.screenBar);
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
    }    

    [SerializeField]
    private ScreenState state;

    [SerializeField]
    private HudOverlayState hudOverlay;

    [SerializeField]
    private HudOverlayMapping[] hudOverlays;

    [SerializeField]
    private ScreenBar screenBar;

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
            overlay.Init();

            var overlayActive = HudOverlay == overlay.State;
            var screenActive = State == overlay.ScreenState
                || overlay.ScreenState == ScreenState.None;

            overlay.Overlay.gameObject.SetActive(overlayActive && screenActive);
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

    public void SetStates(HudOverlayState hudState, ScreenState state)
    {
        this.hudOverlay = hudState;
        this.state = state;
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
﻿#pragma warning disable 0649
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class GUIController : MonoBehaviour
{
    private class GUIControllerTransition : GUITransition
    {
        public GUITransitionProgress progress = GUITransitionProgress.Waiting;
        public ScreenID toScreen;

        public override GUITransitionProgress Progress
        {
            get { return progress; }
        }
    }

    private class LoadingGUITransition : GUITransition
    {
        public GUITransitionProgress progress = GUITransitionProgress.Waiting;

        public override GUITransitionProgress Progress
        {
            get { return progress; }
        }
    }

    private static readonly int StatusBarParamName = Animator.StringToHash("StatusBar");
    private static readonly int HeaderBarParamName = Animator.StringToHash("HeaderBar");

    public static GUIController Current { get; private set; }

    private List<GUIScreen> screens;
    
    private GUIControllerTransition activeTransition;
    private LoadingGUITransition loadingTransition;

    [Header("UI Elements")]

    [SerializeField]
    private CutsceneOverlay cutsceneOverlay;

    [SerializeField]
    private GUIElement loadingOverlay;

    [SerializeField]
    private GUIElement header;

    [SerializeField]
    private Text headerLabel;

    [SerializeField]
    private GUIElement statusBar;

    public bool HasTransition
    {
        get { return activeTransition != null; }
    }

    public ScreenID ActiveScreen
    {
        get
        {
            var activeScreen = FindActiveScreen();
            Debug.Assert(!!activeScreen, "there must be an active screen");
            return activeScreen.ID;
        }
    }

    public CutsceneOverlay CutsceneOverlay
    {
        get { return cutsceneOverlay; }
    }
        
    private GUIScreen FindScreen(ScreenID screenId)
    {
        var screen = screens.Where(s => s.ID == screenId)
            .FirstOrDefault();
        Debug.AssertFormat(!!screen, "screen {0} must exist", screenId);

        return screen;
    }

    private GUIScreen FindActiveScreen()
    {
        var activeScreen = screens.FirstOrDefault(s => s.gameObject.activeSelf);

        return activeScreen;
    }

    private ScreenID DefaultScreen()
    {
        var player = SpaceTraderConfig.LocalPlayer;
        if (player)
        {
            var docked = player.Ship && player.Ship.Moorable.DockedAtStation;

            if (docked)
            {
                return ScreenID.ScreensList;
            }
            else
            {
                return ScreenID.HUD;
            }
        }
        else
        {
            return ScreenID.MainMenu;
        }
    }

    private void OnEnable()
    {        
        Debug.Assert(cutsceneOverlay);
        Debug.Assert(!Current || Current == this);

        Current = this;
        
        screens = new List<GUIScreen>(GetComponentsInChildren<GUIScreen>(true));

        var activeScreen = FindActiveScreen();
        if (activeScreen) 
        {
            header.Activate(activeScreen.ShowHeader);
            statusBar.Activate(activeScreen.ShowStatusBar);
        }
                                
        loadingOverlay.OnTransitionedIn += OnLoadingTransitionedIn;
        loadingOverlay.OnTransitionedOut += OnLoadingTransitionedOut;
        
        DontDestroyOnLoad(gameObject);
    }

    private void OnDisable()
    {
        Debug.Assert(Current == this);
        Current = null;

        loadingOverlay.OnTransitionedIn -= OnLoadingTransitionedIn;
        loadingOverlay.OnTransitionedOut -= OnLoadingTransitionedOut;
    }

    private bool ProcessScreenButton(string button, ScreenID screen)
    {
        if (screen == ScreenID.None || string.IsNullOrEmpty(button) || HasTransition)
        {
            return false;
        }

        if (Input.GetButtonDown(button))
        {
            if (ActiveScreen == screen)
            {
                DismissActive();
            }
            else
            {
                SwitchTo(screen);
            }

            return true;
        }
        return false;
    }

    private void Update()
    {
        var defaultScreen = DefaultScreen();

        if (Input.GetButtonDown("Cancel") && ActiveScreen != defaultScreen)
        {
            DismissActive();
        }
        else
        {
            foreach (var screen in screens)
            {
                if (ProcessScreenButton(screen.ShortcutButton, screen.ID))
                {
                    break;
                }
            }
        }

        if (!screens.Any(s => s.isActiveAndEnabled))
        {
            if (activeTransition == null)
            {
                activeTransition = new GUIControllerTransition();
            }

            if (activeTransition.toScreen == ScreenID.None)
            {
                activeTransition.toScreen = defaultScreen;
            }

            activeTransition.progress = GUITransitionProgress.InProgress;

            var screen = FindScreen(activeTransition.toScreen);
            screen.gameObject.SetActive(true);
            
            header.Activate(screen.ShowHeader);
            statusBar.Activate(screen.ShowStatusBar);
        }

        /* update header if the current screen has a header, and
         we're either not in a transition, or currently transitioning
         into this screen*/
        var activeScreen = FindActiveScreen();
        if (header.Activated
            && (activeTransition == null
                || activeTransition.toScreen == activeScreen.ID))
        {
            var headerText = activeScreen.HeaderText;
            if (string.IsNullOrEmpty(headerText))
            {
                headerText = activeScreen.name.ToUpper();
            }
            headerLabel.text = headerText;
        }
    }

    public GUITransition SwitchTo(ScreenID screen)
    {
        Debug.AssertFormat(activeTransition == null, 
            "there should not already be a transition in progress (already switching to {0})",
            (activeTransition != null)? activeTransition.toScreen : ScreenID.None);

        activeTransition = new GUIControllerTransition
        {
            progress = GUITransitionProgress.Waiting,
            toScreen = screen
        };
        
        var nextScreen = FindScreen(screen != ScreenID.None? screen : DefaultScreen());

        if (!nextScreen.ShowHeader)
        {
            header.Activate(false);
        }
        if (!nextScreen.ShowStatusBar)
        {
            statusBar.Activate(false);
        }

        var activeScreen = FindActiveScreen();
        if (activeScreen)
        {
            activeScreen.Element.Dismiss();
        }
        
        return activeTransition;
    }

    public void DismissActive()
    {
        if (activeTransition != null && activeTransition.toScreen == ScreenID.None)
        {
            return;
        }

        SwitchTo(ScreenID.None);
    }

    private void OnScreenTransitionedOut(GUIScreen screen)
    {
        screen.gameObject.SetActive(false);

        Debug.Assert(!screens.Any(s => s.isActiveAndEnabled));
    }

    private void OnScreenTransitionedIn(GUIScreen screen)
    {
        if (activeTransition != null)
        {
            Debug.Assert(activeTransition.toScreen == screen.ID,
                "must not receive a transitioned-in message while another transition is active");

            activeTransition.progress = GUITransitionProgress.Done;
            activeTransition = null;
        }
    }

    public GUITransition ShowLoadingOverlay()
    {
        loadingOverlay.gameObject.SetActive(true);
        loadingTransition = new LoadingGUITransition
        {
            progress = GUITransitionProgress.InProgress
        };

        return loadingTransition;
    }

    public void DismissLoadingOverlay()
    {
        loadingOverlay.Dismiss();
    }

    private void OnLoadingTransitionedIn()
    {
        Debug.Assert(loadingTransition != null);
        loadingTransition.progress = GUITransitionProgress.Done;
        loadingTransition = null;
    }

    private void OnLoadingTransitionedOut()
    {
        loadingOverlay.gameObject.SetActive(false);
    }
}

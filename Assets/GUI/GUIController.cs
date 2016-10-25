#pragma warning disable 0649
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class GUIController : MonoBehaviour
{
    public class GUIControllerTransition : GUITransition
    {
        public GUITransitionProgress progress = GUITransitionProgress.Waiting;
        public ScreenID toScreen;

        public override GUITransitionProgress Progress
        {
            get { return progress; }
        }
    }

    public class LoadingGUITransition : GUITransition
    {
        public GUITransitionProgress progress = GUITransitionProgress.Waiting;

        public override GUITransitionProgress Progress
        {
            get { return progress; }
        }
    }

    public static GUIController Current { get; private set; }

    private List<GUIScreen> screens;
    
    private GUIControllerTransition activeTransition;
    private LoadingGUITransition loadingTransition;

    [SerializeField]
    private CutsceneOverlay cutsceneOverlay;

    [SerializeField]
    private LoadingOverlay loadingOverlay;

    public ScreenID ActiveScreen
    {
        get { return FindActiveScreen().ID; }
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
        var activeScreen = screens.Where(s => s.isActiveAndEnabled)
            .FirstOrDefault();
        Debug.Assert(!!activeScreen, "there must be an active screen");

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
        
        DontDestroyOnLoad(gameObject);
    }

    private void OnDisable()
    {
        Debug.Assert(Current == this);
        Current = null;
    }
    
    private void Update()
    {
        if (!screens.Any(s => s.isActiveAndEnabled))
        {
            if (activeTransition == null)
            {
                activeTransition = new GUIControllerTransition();
            }

            if (activeTransition.toScreen == ScreenID.None)
            {
                activeTransition.toScreen = DefaultScreen();
            }

            activeTransition.progress = GUITransitionProgress.InProgress;

            var screen = FindScreen(activeTransition.toScreen);
            screen.gameObject.SetActive(true);
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
        
        var activeScreen = FindActiveScreen();        
        activeScreen.Dismiss();

        return activeTransition;
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

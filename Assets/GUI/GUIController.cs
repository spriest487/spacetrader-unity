using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class GUIController : MonoBehaviour
{
    private class TransitionStatus : CustomYieldInstruction
    {
        public GUIController controller;
        public override bool keepWaiting { get { return controller.nextScreen.HasValue; } }
    }

    public static GUIController Current { get; private set; }

    private List<GUIScreen> screens;    
    private ScreenID? nextScreen;

    private TransitionStatus transitionStatus;

    private CutsceneOverlay cutsceneOverlay;

    public ScreenID ActiveScreen
    {
        get { return FindActiveScreen().ID; }
    }

    public CutsceneOverlay CutsceneOveray
    {
        get { return cutsceneOverlay; }
    }

    public void Awake()
    {
        transitionStatus = new TransitionStatus { controller = this };
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

    private void OnEnable()
    {
        Debug.Assert(!Current || Current == this);
        Current = this;
    }

    private void OnDisable()
    {
        Debug.Assert(Current == this);
        Current = null;
    }

    private void Start()
    {
        screens = new List<GUIScreen>(GetComponentsInChildren<GUIScreen>(true));

        cutsceneOverlay = GetComponentInChildren<CutsceneOverlay>();
        Debug.Assert(cutsceneOverlay);

        DontDestroyOnLoad(this.gameObject);
    }
    
    private void Update()
    {
        if (!screens.Any(s => s.isActiveAndEnabled))
        {
            GUIScreen transitionTo;
            if (nextScreen.HasValue)
            {
                transitionTo = FindScreen(nextScreen.Value);
                nextScreen = null;
            }
            else
            {
                transitionTo = FindScreen(ScreenID.MainMenu);
            }

            transitionTo.gameObject.SetActive(true);            
        }
    }

    public CustomYieldInstruction SwitchTo(ScreenID screen)
    {
        if (nextScreen.HasValue)
        {
            Debug.Log("ignoring duplicate call to SwitchTo for " + screen);
        }
        else
        {
            nextScreen = screen;
        
            var activeScreen = FindActiveScreen();        
            activeScreen.Dismiss();
        }

        return transitionStatus;
    }
}

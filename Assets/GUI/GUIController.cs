using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class GUIController : MonoBehaviour
{
    public static GUIController Current { get; private set; }

    private List<GUIScreen> screens;
    
    private GUIScreen nextScreen;

    public ScreenID ActiveScreen
    {
        get { return FindActiveScreen().ID; }
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
    }
    
    private void Update()
    {
        if (!screens.Any(s => s.isActiveAndEnabled))
        {
            GUIScreen transitionTo;
            if (nextScreen)
            {
                transitionTo = nextScreen;
                nextScreen = null;
            }
            else
            {
                transitionTo = FindScreen(ScreenID.MainMenu);
            }

            transitionTo.gameObject.SetActive(true);            
        }
    }

    public void SwitchTo(ScreenID screen)
    {
        if (nextScreen)
        {
            Debug.Log("ignoring duplicate call to SwitchTo for " + screen);
            return;
        }

        nextScreen = FindScreen(screen);

        var activeScreen = FindActiveScreen();        
        activeScreen.Dismiss();
    }
}

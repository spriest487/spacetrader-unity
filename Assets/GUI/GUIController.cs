﻿#pragma warning disable 0649
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.VR;

public class GUIController : MonoBehaviour
{
    public delegate void DismissCallback(Action proceed);

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
    private HeaderBar header;

    [SerializeField]
    private GUIElement statusBar;

    [SerializeField]
    private FollowCamera vrCamera;

    public event DismissCallback OnDismiss;

    public bool HasTransition
    {
        get { return activeTransition != null; }
    }

    public GUIScreen ActiveScreen
    {
        get
        {
            var activeScreen = FindActiveScreen();
            Debug.Assert(!!activeScreen, "there must be an active screen");
            return activeScreen;
        }
    }

    public ScreenID ActiveScreenID { get { return ActiveScreen.ID; } }

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
        var player = Universe.LocalPlayer;
        if (player)
        {
            var docked = player.Dockable && player.Dockable.DockedAtStation;

            if (docked)
            {
                return ScreenID.ScreensList;
            }
            else
            {
                return ScreenID.HUD;
            }
        }
        else if (MissionManager.Instance.Mission)
        {
            return ScreenID.MissionPrep;
        }
        else
        {
            return ScreenID.MainMenu;
        }
    }

    private void OnEnable()
    {
        Current = this;
    }

    private void Start()
    {
        SetupVRMode();

        Debug.Assert(cutsceneOverlay);
        Debug.Assert(!Current || Current == this);

        screens = new List<GUIScreen>(GetComponentsInChildren<GUIScreen>(true));

        var activeScreen = FindActiveScreen();
        if (activeScreen)
        {
            header.Element.Activate(activeScreen.ShowHeader);
            statusBar.Activate(activeScreen.ShowStatusBar);
        }

        loadingOverlay.OnTransitionedIn += OnLoadingTransitionedIn;
        loadingOverlay.OnTransitionedOut += OnLoadingTransitionedOut;

        Universe.OnPrefsSaved += SetupVRMode;
    }

    private void OnDisable()
    {
        Debug.Assert(Current == this);
        Current = null;

        loadingOverlay.OnTransitionedIn -= OnLoadingTransitionedIn;
        loadingOverlay.OnTransitionedOut -= OnLoadingTransitionedOut;

        Universe.OnPrefsSaved -= SetupVRMode;
    }

    private bool ProcessScreenButton(string button, ScreenID screen, ScreenID defaultScreen)
    {
        if (screen == ScreenID.None || string.IsNullOrEmpty(button) || HasTransition)
        {
            return false;
        }

        var buttonState = Input.GetButtonDown(button);

        /*special case: the screen bound to the "cancel" button only activates
        if we're already in the default state, because cancel is also the
        dismiss keybind*/
        if (button == "Cancel")
        {
            buttonState &= screen == defaultScreen;
        }

        if (buttonState)
        {
            if (ActiveScreenID == screen)
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

    private void SetupVRMode()
    {
        const float VR_TARGET_W = 1280f;
        const float VR_TARGET_H = 1024f;
        const float VR_SIZE_W = .30f;
        const float VR_SIZE_H = VR_SIZE_W * (VR_TARGET_H / VR_TARGET_W);

        var canvas = GetComponent<Canvas>();

        if (!VRSettings.enabled)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.transform.SetParent(null, false);
        }
        else
        {
            canvas.renderMode = RenderMode.WorldSpace;

            var canvasXform = canvas.GetComponent<RectTransform>();
            canvasXform.SetParent(vrCamera.WorldSpaceGUIAnchor);

            canvasXform.sizeDelta = new Vector2(VR_TARGET_W, VR_TARGET_H);
            canvasXform.localScale = new Vector3(VR_SIZE_W / VR_TARGET_W, VR_SIZE_H / VR_TARGET_H, 1);
            canvasXform.localPosition = Vector3.zero;
        }
    }

    private void Update()
    {
        var canvas = GetComponent<Canvas>();
        if (canvas.worldCamera != vrCamera.Camera)
        {
            canvas.worldCamera = vrCamera.Camera;
        }

        if (!HasTransition)
        {
            var defaultScreen = DefaultScreen();
            if (Input.GetButtonDown("Cancel")
                && ActiveScreenID != defaultScreen)
            {
                DismissActive();
            }
            else
            {
                foreach (var screen in screens)
                {
                    if (ProcessScreenButton(screen.ShortcutButton, screen.ID, defaultScreen))
                    {
                        break;
                    }
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
                activeTransition.toScreen = DefaultScreen();
            }

            activeTransition.progress = GUITransitionProgress.InProgress;

            var screen = FindScreen(activeTransition.toScreen);
            screen.gameObject.SetActive(true);
        }

        var activeScreen = FindActiveScreen();

        if (activeTransition == null || activeTransition.toScreen == activeScreen.ID)
        {
            /* update header if the current screen has a header, and
            we're either not in a transition, or currently transitioning
            into this screen*/

            header.Element.Activate(activeScreen.ShowHeader);
            statusBar.Activate(activeScreen.ShowStatusBar);
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
            header.Element.Activate(false);
        }
        if (!nextScreen.ShowStatusBar)
        {
            statusBar.Activate(false);
        }

        var activeScreen = FindActiveScreen();
        if (activeScreen)
        {
            if (activeScreen.ID == nextScreen.ID)
            {
                activeTransition.progress = GUITransitionProgress.Done;
                activeTransition = null;
            }
            else
            {
                activeScreen.Element.Dismiss();
            }
        }

        return activeTransition;
    }

    public void DismissActive()
    {
        if (activeTransition != null && activeTransition.toScreen == ScreenID.None)
        {
            return;
        }

        if (OnDismiss != null)
        {
            bool proceeded = false;
            OnDismiss.Invoke(() =>
            {
                if (!proceeded)
                {
                    SwitchTo(ScreenID.None);
                    proceeded = true;
                }
            });
        }
        else
        {
            SwitchTo(ScreenID.None);
        }
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

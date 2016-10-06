#pragma warning disable 0649

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    
    [SerializeField]
    private PlayerStatus playerStatus;
    
    [SerializeField]
    private ScreenID screenId;

    [SerializeField]
    private List<ScreenMapping> screens = new List<ScreenMapping>();

    [SerializeField]
    private LoadingScreen loadingScreen;
        
    [SerializeField]
    private bool menuState;

    [SerializeField]
    private Cutscene cutscene;

    [SerializeField]
    private CanvasGroup fullscreenFadeOverlay;
    
    private Coroutine currentFullscreenFade;
    public bool FadeTransitionInProgress
    {
        get { return currentFullscreenFade != null; }
    }

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

    public ScreenID ScreenID
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

    public void BroadcastScreenMessage(PlayerStatus playerStatus,
        ScreenID overlayState,
        string message, 
        object value)
    {
        foreach (var screen in screens)
        {
            if (screen.PlayerStatus == playerStatus && screen.ScreenID == overlayState)
            {
                screen.Root.BroadcastMessage(message, value, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    public void BroadcastPlayerNotification(string message)
    {
        BroadcastScreenMessage(PlayerStatus.Flight, ScreenID.None, "OnPlayerNotification", message);
    }
    
    private void Apply()
    {
        foreach (var screen in screens)
        {
            screen.Init();

            var screenIdMatches = ScreenID == screen.ScreenID;
            var playerStatusMatches = State == screen.PlayerStatus
                || screen.PlayerStatus == PlayerStatus.None;

            var screenActive = screenIdMatches && playerStatusMatches;
            
            screen.Root.gameObject.SetActive(screenActive);

            screen.Root.BroadcastMessage(screenActive ? "OnScreenActive" : "OnScreenInactive", SendMessageOptions.DontRequireReceiver);
        }
    }

    public void ToggleOverlay(ScreenID state)
    {
        if (ScreenID == state)
        {
            ScreenID = DefaultHudOverlay;
        }
        else
        {
            ScreenID = state;
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
        screenId = DefaultHudOverlay;
        playerStatus = PlayerStatus.Flight;

        Apply();
    }

    private ScreenMapping FindCurrentScreen()
    {
        Debug.Assert(screens.Where(s => s.Root.activeInHierarchy).Count() <= 1);

        for (int s = 0; s < screens.Count; ++s)
        {
            if (screens[s].Root.activeInHierarchy)
            {
                return screens[s];
            }
        }

        return null;
    }
    
    private void Update()
    {
        bool docked = false;

        var player = PlayerShip.LocalPlayer;
        if (player)
        {
            var moorable = player.Moorable;
            if (moorable && moorable.State == DockingState.Docked)
            {
                docked = true;
            }
        }

        State = docked? PlayerStatus.Docked : PlayerStatus.Flight;

        if (player)
        {
            /* the player can switch to any of these screens at any
             time with these global keys */
            foreach (var screen in screens)
            {
                if (!string.IsNullOrEmpty(screen.HotkeyButton)
                    && Input.GetButtonDown(screen.HotkeyButton))
                {
                    var toScreen = ScreenID != screen.ScreenID ? screen.ScreenID : ScreenID.None;
                    
                    TryFadeScreenTransition(toScreen,
                        screen.TransitionIn,
                        screen.TransitionOut);
                }
            }

            if (Input.GetButtonDown("Cancel"))
            {
                switch (ScreenID)
                {
                    case ScreenID.None:
                        TryFadeScreenTransition(ScreenID.MainMenu);
                        break;
                    default:
                        var currentScreen = FindCurrentScreen();

                        TryFadeScreenTransition(ScreenID.None, 
                            currentScreen.TransitionIn, 
                            currentScreen.TransitionOut);
                        break;
                }
            }
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        Start();
    }

    public LoadingScreen CreateLoadingScreen()
    {
        var instance = Instantiate(loadingScreen);

        DontDestroyOnLoad(instance);
        instance.gameObject.SetActive(true);

        return instance;
    }
    
    private void SetFadeElementsAlpha(ScreenTransition transition, float t)
    {
        if (transition.IsInvertDirection())
        {
            t = 1 - t;
        }

        //if overlay is active, that's what should be animated
        float guiAlpha;
        if (fullscreenFadeOverlay.gameObject.activeInHierarchy)
        {
            fullscreenFadeOverlay.alpha = t;
            guiAlpha = 1; 
        }
        else
        {
            guiAlpha = t;
        }

        foreach (var screen in screens)
        {
            screen.CanvasGroup.alpha = guiAlpha;
        }
    }

    private IEnumerator FullscreenFadeRoutine(ScreenTransition transition, Action onFinish)
    {
        const float DURATION = 0.075f;
        float start = Time.time;
        float end = start + DURATION;
        float now = start;
        
        fullscreenFadeOverlay.gameObject.SetActive(transition.IsShowOverlay());

        do
        {
            float t = Mathf.Clamp01((now - start) / DURATION);
            
            SetFadeElementsAlpha(transition, t);            

            yield return null;
        }
        while ((now = Time.time) < end);

        //make sure this finishes in the 100% state
        SetFadeElementsAlpha(transition, 1);

        fullscreenFadeOverlay.gameObject.SetActive(transition.IsShowOverlayAfter());
        
        currentFullscreenFade = null;

        if (onFinish != null)
        {
            onFinish();
        }
    }

    public void FullScreenFade(ScreenTransition transitionType,
        Action onFinish = null)
    {
        if (!fullscreenFadeOverlay)
        {
            var overlay = new GameObject("Fade Overlay");
            overlay.transform.SetAsLastSibling();

            var canvas = overlay.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var image = overlay.AddComponent<RawImage>();
            image.color = Color.black;
            image.texture = null;

            fullscreenFadeOverlay = overlay.AddComponent<CanvasGroup>();
        }

        Debug.Assert(currentFullscreenFade == null, "can't start a fade transition while one is already active");

        currentFullscreenFade = StartCoroutine(FullscreenFadeRoutine(transitionType, onFinish));
    }

    public bool TryFadeScreenTransition(ScreenID screenId,
        ScreenTransition transitionIn = ScreenTransition.FadeOutAlpha,
        ScreenTransition transitionOut = ScreenTransition.FadeInAlpha,
        Action onFinish = null)
    {
        return TryFadeScreenTransition(PlayerStatus.None, screenId, transitionIn, transitionOut, onFinish);
    }

    public bool TryFadeScreenTransition(PlayerStatus playerStatus,
        ScreenID screenId,        
        ScreenTransition transitionIn = ScreenTransition.FadeOutAlpha, 
        ScreenTransition transitionOut = ScreenTransition.FadeInAlpha,
        Action onFinish = null)
    {
        if (FadeTransitionInProgress)
        {
            Debug.Log("prevented fade transition, another transition was already in progress");
            return false;
        }

        FullScreenFade(transitionIn, () =>
        {
            this.screenId = screenId;

            if (playerStatus != PlayerStatus.None)
            {
                this.playerStatus = playerStatus;
            }

            Apply();
            if (onFinish != null)
            {
                onFinish();
            }

            FullScreenFade(transitionOut);            
        });

        return true;
    }
}
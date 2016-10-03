#pragma warning disable 0649

using System;
using System.Collections.Generic;
using System.Collections;
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

    [Serializable]
    private class ScreenMapping
    {
        [SerializeField]
        private ScreenID screenId;

        [SerializeField]
        private PlayerStatus playerStatus;
        
        [SerializeField]
        private GameObject root;
        
        [HideInInspector]
        [SerializeField]
        private GameObject overlayInstance;
        
        public GameObject Root { get { return overlayInstance; } }

        public ScreenID ScreenID { get { return screenId; } }
        public PlayerStatus PlayerStatus { get { return playerStatus; } }
        
        public void Init()
        {
            if (!overlayInstance)
            {
                overlayInstance = Instantiate(root);
                
                DontDestroyOnLoad(overlayInstance.gameObject);
            }
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

    private class FullscreenFadeEffect
    {
        public bool Reverse;
        public Coroutine Coroutine;
        public Action OnFinish;
    }

    private FullscreenFadeEffect currentFullscreenFade;

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

    private IEnumerator FullscreenFadeRoutine()
    {
        const float DURATION = 0.1f;
        float start = Time.time;
        float end = start + DURATION;
        float now = start;

        fullscreenFadeOverlay.gameObject.SetActive(true);

        do
        {
            float t = Mathf.Clamp01((now - start) / DURATION);

            if (currentFullscreenFade.Reverse)
            {
                t = 1 - t;
            }

            fullscreenFadeOverlay.alpha = t;
            yield return null;
        }
        while ((now = Time.time) < end);

        fullscreenFadeOverlay.gameObject.SetActive(currentFullscreenFade.Reverse);

        var onFinish = currentFullscreenFade.OnFinish;
        currentFullscreenFade = null;

        if (onFinish != null)
        {
            onFinish();
        }
    }

    public void FullScreenFade(bool reverse, Action onFinish = null)
    {
        if (!fullscreenFadeOverlay)
        {
            var overlay = new GameObject("Fade Overlay");
            var canvas = overlay.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var image = overlay.AddComponent<RawImage>();
            image.color = Color.black;
            image.texture = null;

            fullscreenFadeOverlay = overlay.AddComponent<CanvasGroup>();
        }

        Debug.Assert(currentFullscreenFade == null);

        currentFullscreenFade = new FullscreenFadeEffect()
        {
            Reverse = reverse,
            OnFinish = onFinish
        };
        currentFullscreenFade.Coroutine = StartCoroutine(FullscreenFadeRoutine());
    }

    public void FadeScreenTransition(ScreenID screenId, PlayerStatus playerStatus = PlayerStatus.None)
    {
        FullScreenFade(false, () =>
        {
            this.screenId = screenId;

            if (playerStatus != PlayerStatus.None)
            {
                this.playerStatus = playerStatus;
            }

            Apply();
        });
    }
}
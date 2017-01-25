#pragma warning disable 0649

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VR;
using System;
using System.Collections;

public class SpaceTraderConfig : MonoBehaviour
{
    public static SpaceTraderConfig Instance { get; private set; }

    public static QuestBoard QuestBoard { get { return Instance.questBoard; } }
    public static CrewConfiguration CrewConfiguration { get { return Instance.crewConfig; } }
    public static CargoItemConfiguration CargoItemConfiguration { get { return Instance.cargoConfig; } }
    public static MissionsConfiguration MissionsConfiguration { get { return Instance.missionsConfig; } }
    public static Market Market { get { return Instance.market; } }
    public static FleetManager FleetManager { get { return Instance? Instance.fleetManager : null; } }
    public static WorldMap WorldMap { get { return Instance.worldMap; } }

    public static Scene GlobalScene { get { return SceneManager.GetSceneByBuildIndex(0); } }

    public static PlayerShip LocalPlayer
    {
        get { return Instance.localPlayer; }
        set
        {
            /* making a player the local player makes them global */
            if (value)
            {
                SceneManager.MoveGameObjectToScene(value.gameObject, GlobalScene);
            }

            Instance.localPlayer = value;

            if (OnLocalPlayerChanged != null)
            {
                OnLocalPlayerChanged.Invoke();
            }
        }
    }

    public static bool TouchControlsEnabled { get; set; }

    [SerializeField]
    private QuestBoard questBoard;

    [SerializeField]
    private CrewConfiguration crewConfig;

    [SerializeField]
    private CargoItemConfiguration cargoConfig;

    [SerializeField]
    private MissionsConfiguration missionsConfig;

    [SerializeField]
    private Market market;

    [SerializeField]
    private PlayerShip localPlayer;

    [SerializeField]
    private FleetManager fleetManager;

    [SerializeField]
    private WorldMap worldMap;
   
    public static event Action OnPrefsSaved;
    public static event Action OnLocalPlayerChanged;
        
    private void OnEnable()
    {
        Instance = this;

        MissionManager.OnMissionChanged += mission =>
        {
            if (mission)
            {
                LocalPlayer = null;
            }
            else
            {
                LocalPlayer = FindObjectOfType<PlayerShip>();
            }
        };

        SceneManager.activeSceneChanged += (oldScene, newScene) =>
        {
            PlayerNotifications.Clear();
        };

        //apply initial settings
        ReloadPrefs();
    }

    private void Awake()
    {
        //clone configs on startup so we don't modify the global assets
        questBoard = QuestBoard.Create(questBoard);
        crewConfig = CrewConfiguration.Create(crewConfig);
        cargoConfig = Instantiate(cargoConfig);
        missionsConfig = Instantiate(missionsConfig);
        market = Instantiate(market);
        fleetManager = Instantiate(fleetManager);

        Debug.Assert(worldMap);
    }

    public static void SavePrefs()
    {
        PlayerPrefs.SetInt("VR Enabled", VRSettings.enabled? 1 : 0);
        PlayerPrefs.SetInt("Touch Controls Enabled", TouchControlsEnabled? 1 : 0);
        PlayerPrefs.Save();

        if (OnPrefsSaved != null)
        {
            OnPrefsSaved.Invoke();
        }
    }

    public static void ReloadPrefs()
    {
        var vrEnabledPref = PlayerPrefs.GetInt("VR Enabled", 0);
        Instance.StartCoroutine(Instance.EnableVR(vrEnabledPref == 1));

        TouchControlsEnabled = PlayerPrefs.GetInt("Touch Controls Enabled", 0) == 1;
    }

    private void Update()
    {
        //global shortcut for changing vr mode
        if (Input.GetButtonDown("Reset Orientation") &&
            (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
        {
            StartCoroutine(EnableVR(!VRSettings.enabled));
        }
    }

    private IEnumerator EnableVR(bool enable)
    {
        if (!enable)
        {
            VRSettings.enabled = false;
            yield break;
        }

        VRSettings.LoadDeviceByName(VRSettings.supportedDevices);
        yield return null;

        VRSettings.enabled = true;
        if (VRSettings.enabled)
        {
            Debug.Log("initialized VR with device " +VRSettings.loadedDeviceName);
        }
        else
        {
            Debug.LogWarning("failed to initialize VR (no devices could be enabled)");
        }
    }
}
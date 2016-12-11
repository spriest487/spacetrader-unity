#pragma warning disable 0649

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VR;
using System;

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
        }
    }

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

        SceneManager.activeSceneChanged += (oldScene, newScene) =>
        {
            PlayerNotifications.Clear();
        };

        //apply initial settings
        ReloadPrefs();
    }

    public static void SavePrefs()
    {
        PlayerPrefs.SetInt("VR Enabled", VRSettings.enabled? 1 : 0);
        PlayerPrefs.Save();

        if (OnPrefsSaved != null)
        {
            OnPrefsSaved.Invoke();
        }
    }

    public static void ReloadPrefs()
    {
        VRSettings.enabled = PlayerPrefs.GetInt("VR Enabled", 0) == 1;
    }

    private void Update()
    {
        //global shortcut for changing vr mode
        if (Input.GetButtonDown("Reset Orientation") &&
            (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
        {
            VRSettings.enabled = !VRSettings.enabled;

            SavePrefs();
        }
    }
}
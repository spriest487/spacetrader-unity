#pragma warning disable 0649

using UnityEngine;
using UnityEngine.SceneManagement;

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

    public static PlayerShip LocalPlayer
    {
        get { return Instance.localPlayer; }
        set
        {
            /* making a player the local player makes them global */
            if (value)
            {
                var globals = SceneManager.GetSceneByBuildIndex(0);
                SceneManager.MoveGameObjectToScene(value.gameObject, globals);
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

    private void Awake()
    {
        Instance = this;

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

        FindObjectOfType<MissionManager>().OnMissionChanged += mission =>
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
}
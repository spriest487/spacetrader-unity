#pragma warning disable 0649

using UnityEngine;

public class SpaceTraderConfig : MonoBehaviour
{
    public static SpaceTraderConfig Instance { get; private set; }

    public static CrewConfiguration CrewConfiguration { get { return Instance.crewConfig; } }
    public static CargoItemConfiguration CargoItemConfiguration { get { return Instance.cargoConfig; } }
    public static MissionsConfiguration MissionsConfiguration { get { return Instance.missionsConfig; } }
    public static Market Market { get { return Instance.market; } }
    public static FleetManager FleetManager { get { return Instance? Instance.fleetManager : null; } }

    public static PlayerShip LocalPlayer
    {
        get { return Instance.localPlayer; }
        set
        {
            /* making a player the local player makes them persist through
            level changes too */
            if (value)
            {
                DontDestroyOnLoad(value);
            }

            Instance.localPlayer = value;
        }
    }
    
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
    
    private void OnEnable()
    {
        if (Instance)
        {
            Debug.Assert(Instance == this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);

        //clone configs on startup so we don't modify the global assets
        crewConfig = CrewConfiguration.Create(crewConfig);
        cargoConfig = Instantiate(cargoConfig);
        missionsConfig = Instantiate(missionsConfig);
        market = Instantiate(market);
        fleetManager = Instantiate(fleetManager);
    }
    
    private void OnDisable()
    {
        Instance = null;
    }

    private void OnDestroy()
    {
        Destroy(crewConfig);
        Destroy(cargoConfig);
        Destroy(missionsConfig);
        Destroy(market);
        Destroy(fleetManager);
    }

    private void OnLevelWasLoaded()
    {
        /*bit of a hack, try to find the player (this won't actually
            get used since we will usually have a mission config) */
        if (!MissionManager.Instance)
        {
            localPlayer = FindObjectOfType<PlayerShip>();
        }
    }
}
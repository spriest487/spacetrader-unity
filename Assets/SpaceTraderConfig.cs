using UnityEngine;

public class SpaceTraderConfig : MonoBehaviour
{
    private static SpaceTraderConfig instance;
    public static SpaceTraderConfig Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SpaceTraderConfig>();
            }

            return instance;
        }
        private set
        {
            instance = value;
        }
    }

    public static CrewConfiguration CrewConfiguration { get { return Instance.crewConfig; } }
    public static CargoItemConfiguration CargoItemConfiguration { get { return Instance.cargoConfig; } }
    public static MissionsConfiguration MissionsConfiguration { get { return Instance.missionsConfig; } }
    public static Market Market { get { return Instance.market; } }
    public static FleetManager FleetManager { get { return Instance.fleetManager; } }
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
        if (instance == this)
        {
            instance = null;
        }
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
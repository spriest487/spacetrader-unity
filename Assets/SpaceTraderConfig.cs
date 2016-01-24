using UnityEngine;

public class SpaceTraderConfig : MonoBehaviour
{
    public static SpaceTraderConfig Instance { get; private set; }
    
    public static CrewConfiguration CrewConfiguration { get { return Instance.crewConfig; } }
    public static CargoItemConfiguration CargoItemConfiguration { get { return Instance.cargoConfig; } }
    public static MissionsConfiguration MissionsConfiguration { get { return Instance.missionsConfig; } }
    public static Market Market { get { return Instance.market; } }
    
    [SerializeField]
    private CrewConfiguration crewConfig;

    [SerializeField]
    private CargoItemConfiguration cargoConfig;

    [SerializeField]
    private MissionsConfiguration missionsConfig;

    [SerializeField]
    private Market market;
    
    private void OnEnable()
    {
        Instance = this;
    }
    
    private void OnDisable()
    {
        Destroy(Instance);
    }
}
using UnityEditor;

public static class ConfigSingletonMenus
{
    [MenuItem("Assets/Create/SpaceTrader/Items/Item configuration")]
    public static void CargoItemsConfig()
    {
        ScriptableObjectUtility.CreateAsset<CargoItemConfiguration>();
    }

    [MenuItem("Assets/Create/SpaceTrader/Missions/Missions configuration")]
    public static void MissionsConfig()
    {
        ScriptableObjectUtility.CreateAsset<MissionsConfiguration>();
    }

    [MenuItem("Assets/Create/SpaceTrader/Crew/Crew configuration")]
    public static void CrewConfig()
    {
        ScriptableObjectUtility.CreateAsset<CrewConfiguration>();
    }

    [MenuItem("Assets/Create/SpaceTrader/Fleet manager")]
    public static void FleetManager()
    {
        ScriptableObjectUtility.CreateAsset<FleetManager>();
    }
}
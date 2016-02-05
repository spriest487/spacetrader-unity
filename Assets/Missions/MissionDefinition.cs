using UnityEngine;

public class MissionDefinition : ScriptableObject
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Create/SpaceTrader/Missions/Mission definition")]
    public static void CreateMissionDefiniton()
    {
        ScriptableObjectUtility.CreateAsset<MissionDefinition>();
    }
#endif

    [System.Serializable]
    public class PlayerSlot
    {
        [SerializeField]
        private ShipType shipType;

        [SerializeField]
        private ModulePreset modulePreset;

        [SerializeField]
        private string name;

        public ShipType ShipType { get { return shipType; } }
        public ModulePreset ModulePreset { get { return modulePreset; } }
        public string Name { get { return name; } }

        public Ship SpawnShip(Vector3 pos, Quaternion rot, TeamDefinition team)
        {
            var ship = ShipType.CreateShip(pos, rot);
            ship.name = Name;

            var targetable = ship.gameObject.AddComponent<Targetable>();
            targetable.Faction = team.Name;

            if (modulePreset)
            {
                modulePreset.Apply(ship);
            }

            return ship;
        }
    }

    [System.Serializable]
    public class TeamDefinition
    {
        [SerializeField]
        private string name;

        [SerializeField]
        private PlayerSlot[] slots;

        public string Name { get { return name; } }
        public PlayerSlot[] Slots { get { return slots; } }
    }

    [SerializeField]
    private string sceneName;

    [SerializeField]
    private string missionName;

    [TextArea]
    [SerializeField]
    private string description;

    [SerializeField]
    private TeamDefinition[] teams;

    public string SceneName { get { return sceneName; } }
    public string MissionName { get { return missionName; } }
    public string Description { get { return description; } }

    public TeamDefinition[] Teams { get { return teams; } }
    
    public void LoadMission()
    {
        Application.LoadLevel(SceneName);
    }
}
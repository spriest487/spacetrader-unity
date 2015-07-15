using UnityEngine;

public class MissionDefinition : ScriptableObject
{
    [System.Serializable]
    public class PlayerSlot
    {
        [SerializeField]
        private ShipType shipType;

        public ShipType ShipType { get { return shipType; } }
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

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Create/SpaceTrader/Mission Definition")]
    public static void CreateMissionDefiniton() {
        ScriptableObjectUtility.CreateAsset<MissionDefinition>();
    }
#endif

    public void LoadMission()
    {
        Application.LoadLevel(SceneName);
    }
}
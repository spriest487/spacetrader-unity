using UnityEngine;

public class MissionDefinition : ScriptableObject
{
    public enum SlotStatus
    {
        Open,
        Closed,
        Human,
        AI
    }

    [System.Serializable]
    public class PlayerSlot
    {
        [SerializeField]
        private SlotStatus status;

        [SerializeField]
        private ShipType shipType;

        public SlotStatus Status { get { return status; } }
        public ShipType ShipType { get { return shipType; } }
    }

    [SerializeField]
    private string sceneName;

    [SerializeField]
    private string missionName;

    [TextArea]
    [SerializeField]
    private string description;

    [SerializeField]
    private PlayerSlot[] playerSlots;

    public string SceneName { get { return sceneName; } }
    public string MissionName { get { return missionName; } }
    public string Description { get { return description; } }

    public PlayerSlot[] PlayerSlots { get { return playerSlots; } }

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
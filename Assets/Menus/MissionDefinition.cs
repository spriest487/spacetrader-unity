using UnityEngine;

public class MissionDefinition : ScriptableObject
{
    [SerializeField]
    private string sceneName;

    [SerializeField]
    private string missionName;

    [TextArea]
    [SerializeField]
    private string description;

    public string SceneName { get { return sceneName; } }
    public string MissionName { get { return missionName; } }
    public string Description { get { return description; } }

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
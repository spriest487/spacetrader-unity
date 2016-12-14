#pragma warning disable 0649

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "SpaceTrader/Missions/Mission Definition")]
public class MissionDefinition : ScriptableObject
{
    [SerializeField]
    private string sceneName;

    [SerializeField]
    private string missionName;

    [TextArea]
    [SerializeField]
    private string description;

    [SerializeField]
    private Sprite image;

    [SerializeField]
    private List<TeamDefinition> teams;

    public string SceneName { get { return sceneName; } }
    public string MissionName { get { return missionName; } }
    public string Description { get { return description; } }
    public Sprite Image { get { return image; } }

    public IEnumerable<TeamDefinition> Teams { get { return teams; } }
    public int TeamCount { get {return teams.Count; } }

    public void LoadMission()
    {
        SceneManager.LoadScene(SceneName);
    }

    public TeamDefinition GetTeam(int team)
    {
        return teams[team];
    }
}

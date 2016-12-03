#pragma warning disable 0649

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName = "SpaceTrader/Missions Config")]
public class MissionsConfiguration : ScriptableObject
{
    [SerializeField]
    private List<MissionDefinition> missions;

    public List<MissionDefinition> Missions { get { return missions; } }

    public MissionDefinition MissionForScene(Scene scene)
    {
        return missions.Where(m => m.SceneName == scene.name).FirstOrDefault();
    }
}
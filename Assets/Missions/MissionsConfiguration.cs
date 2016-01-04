using UnityEngine;
using System.Collections.Generic;

public class MissionsConfiguration : ScriptableObject
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Create/SpaceTrader/Missions/Missions configuration")]
    public static void CreateNewConfiguration()
    {
        ScriptableObjectUtility.CreateAsset<MissionsConfiguration>();
    }
#endif

    [SerializeField]
    private List<MissionDefinition> missions;

    public List<MissionDefinition> Missions { get { return missions; } }
}
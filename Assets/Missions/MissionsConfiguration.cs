using UnityEngine;
using System.Collections.Generic;

public class MissionsConfiguration : ScriptableObject
{
#if UNITY_EDITOR
    
#endif

    [SerializeField]
    private List<MissionDefinition> missions;

    public List<MissionDefinition> Missions { get { return missions; } }
}
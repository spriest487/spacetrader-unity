using UnityEngine;
using System.Collections.Generic;

public class MissionsConfiguration : ScriptableObject
{
    [SerializeField]
    private List<MissionDefinition> missions;

    public List<MissionDefinition> Missions { get { return missions; } }
}
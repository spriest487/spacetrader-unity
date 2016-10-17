#pragma warning disable 0649

using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "SpaceTrader/Missions Config")]
public class MissionsConfiguration : ScriptableObject
{
    [SerializeField]
    private List<MissionDefinition> missions;

    public List<MissionDefinition> Missions { get { return missions; } }
}
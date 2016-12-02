#pragma warning disable 0649

using UnityEngine;
using System;

public class ActiveMission : ScriptableObject
{
    public const string MISSION_TAG = "MissionObjective";

    [SerializeField]
    private MissionDefinition missionDefinition;
    
    [SerializeField]
    private ActiveTeam[] teams;
    
    public MissionDefinition Definition { get { return missionDefinition; } }
    
    public ActiveTeam[] Teams { get { return teams; } }
    
    public static ActiveMission Create(MissionDefinition definition)
    {
        var result = CreateInstance<ActiveMission>();

        result.missionDefinition = definition;
        result.teams = new ActiveTeam[definition.Teams.Count];
        result.name = definition.name;

        bool firstSlot = true;

        for (int team = 0; team < result.teams.Length; ++team)
        {
            var newTeam = new ActiveTeam(definition.Teams[team]);
            
            foreach (var slot in newTeam.Slots)
            {
                slot.Status = firstSlot ? SlotStatus.Human : SlotStatus.AI;
                firstSlot = false;
            }

            result.teams[team] = newTeam;
        }

        return result;
    }
}

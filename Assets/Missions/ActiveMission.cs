#pragma warning disable 0649

using UnityEngine;
using System;

[Serializable]
public class ActiveMission
{
    public const string MISSION_TAG = "MissionObjective";

    [SerializeField]
    private MissionDefinition missionDefinition;
    
    [SerializeField]
    private ActiveTeam[] teams;
    
    public MissionDefinition Definition { get { return missionDefinition; } }
    
    public ActiveTeam[] Teams { get { return teams; } }
    
    public void Init()
    {
        teams = new ActiveTeam[Definition.Teams.Count];

        bool firstSlot = true;

        for (int team = 0; team < teams.Length; ++team)
        {
            var newTeam = new ActiveTeam(Definition.Teams[team]);
            
            foreach (var slot in newTeam.Slots)
            {
                slot.Status = firstSlot ? SlotStatus.Human : SlotStatus.AI;
                firstSlot = false;
            }

            teams[team] = newTeam;
        }
    }
}

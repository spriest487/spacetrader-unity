using UnityEngine;
using System;

[Serializable]
public class ActiveMission
{
    public const string MISSION_TAG = "MissionObjective";

    [SerializeField]
    private MissionDefinition missionDefinition;

    [SerializeField]
    private string[] winningTeams;

    [SerializeField]
    private ActiveTeam[] teams;
    
    public MissionDefinition Definition { get { return missionDefinition; } }

    public string[] WinningTeams
    {
        get { return winningTeams; }
        set { winningTeams = (string[])value.Clone(); }
    }

    public ActiveTeam[] Teams { get { return teams; } }

    public static MissionObjective[] FindObjectives(string team)
    {
        var allObjectives = GameObject.FindGameObjectsWithTag(MISSION_TAG);

        var teamCount = 0;
        var teamObjectives = new MissionObjective[allObjectives.Length];

        foreach (var obj in allObjectives)
        {
            var objective = obj.GetComponent<MissionObjective>();

            foreach (var objectiveTeam in objective.Teams)
            {
                if (objectiveTeam == team)
                {
                    teamObjectives[teamCount] = objective;
                    teamCount++;
                }
            }
        }

        var result = new MissionObjective[teamCount];
        for (int objIndex = 0; objIndex < teamCount; ++objIndex)
        {
            result[objIndex] = teamObjectives[objIndex];
        }

        return result;
    }

    public void Init()
    {
        teams = new ActiveTeam[Definition.Teams.Length];

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

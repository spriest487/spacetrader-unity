#pragma warning disable 0649

using UnityEngine;
using System.Collections.Generic;

public class TeamSpawner : MonoBehaviour
{
    [SerializeField]
    private bool spawnOnMissionStart = true;

    [SerializeField]
    private string spawnTag;

    [SerializeField]
    private List<TeamSpawnerTeam> teams;

    public IEnumerable<TeamSpawnerTeam> Teams { get { return teams; } }

    private TeamSpawnerTeam FindTeam(string name)
    {
        foreach (var team in teams)
        {
            if (team.Name == name)
            {
                return team;
            }
        }

        return null;
    }

    private void OnEnable()
    {
        MissionManager.OnPhaseChanged += OnPhaseChanged;
    }

    private void OnDisable()
    {
        MissionManager.OnPhaseChanged -= OnPhaseChanged;
    }

    private void OnPhaseChanged(MissionPhase phase)
    {
        var mission = MissionManager.Instance.Mission;

        switch (phase)
        {
            case MissionPhase.Active:
                if (spawnOnMissionStart)
                for (int team = 0; team < mission.Teams.Length; ++team)
                {
                    var teamDefinition = mission.Definition.GetTeam(team);
                    var activeTeam = mission.Teams[team];

                    var spawnedTeam = FindTeam(teamDefinition.Name);

                    if (spawnedTeam != null)
                    {
                        spawnedTeam.SpawnAll(teamDefinition, activeTeam, spawnTag);
                    }
                }
                break;
        }
    }
}

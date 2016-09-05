#pragma warning disable 0649

using System;
using UnityEngine;
using System.Linq;

public class KillEnemyTeamQuest : Quest
{
    [SerializeField]
    private int targetTeamIndex;

    private ActiveTeam GetTeam()
    {
        return MissionManager.Instance.Mission.Teams[targetTeamIndex];
    }
    
    public override string Description
    {
        get
        {
            var teamName = GetTeam().Definition.Name;
            return string.Format("Destroy all ships on the {0} team", teamName);
        }
    }

    public override bool Done
    {
        get
        {
            return !GetTeam().Slots.Any(s => s.SpawnedShip);
        }
    }

    public override int MoneyReward
    {
        get { return 0; }
    }

    public override int XPReward
    {
        get { return 0; }
    }
}
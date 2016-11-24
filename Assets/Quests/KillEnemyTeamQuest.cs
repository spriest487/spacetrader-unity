#pragma warning disable 0649

using System;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "SpaceTrader/Quests/Kill Enemey Team Quest")]
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

    public override QuestStatus Status
    { 
        get
        {
            if (!GetTeam().Slots.Any(s => s.SpawnedShip))
            {
                return QuestStatus.Completed;
            }
            else
            {
                return base.Status;
            }
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
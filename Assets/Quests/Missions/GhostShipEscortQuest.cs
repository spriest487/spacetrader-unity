using UnityEngine;
using System.Collections;
using System;

[CreateAssetMenu(menuName = "SpaceTrader/Quests/Ghost Ship Escort Quest")]
public class GhostShipEscortQuest : Quest
{
    bool failed;

    private GhostShipAI ghostShip;

    public override string Description
    {
        get
        {
            return "Ghost Ship must reach the destination marker";
        }
    }

    public override QuestStatus Status
    {
        get
        {
            if (failed)
            {
                return QuestStatus.Failed;
            }
            else if (ghostShip && ghostShip.Ship.IsCloseTo(ghostShip.Goal.position))
            {
                return QuestStatus.Completed;
            }
            else
            {
                return base.Status;
            }
        }
    }

    public override void OnAccepted()
    {
        var ship = FindObjectOfType<GhostShipAI>();
        Debug.Assert(ship && ship.Goal, "must be a Ghost Ship AI in the scene with a valid goal");
    }

    public override void NotifyDeath(Ship ship, Ship killer)
    {
        if (ship.name == "Ghost Ship")
        {
            failed = true;
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

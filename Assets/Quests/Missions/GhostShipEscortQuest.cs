using UnityEngine;
using System.Collections;
using System;

[CreateAssetMenu(menuName = "SpaceTrader/Quests/Ghost Ship Escort Quest")]
public class GhostShipEscortQuest : Quest
{
    bool failed;

    private Ship ghostShip;
    private Transform goal;

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
            else if (ghostShip && ghostShip.IsCloseTo(goal.transform.position))
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
        var shipObj = GameObject.Find("Ghost Ship");
        ghostShip = shipObj ? shipObj.GetComponent<Ship>() : null;
        Debug.Assert(ghostShip, "must be a ship named Ghost Ship in the scene");

        var goalObj = GameObject.Find("Ghost Ship Goal");
        Debug.Assert(goal, "must be an object named Ghost Ship Goal in the scene");
        goal = goalObj.transform;
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

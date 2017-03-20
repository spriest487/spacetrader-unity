#pragma warning disable 0649

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public enum WingmanOrder
{
    FollowLeader = RadioMessageType.FollowMe,
    Wait = RadioMessageType.Wait,
    AttackLeaderTarget = RadioMessageType.Attack,
}

public static class WingmanOrderUtility
{
    public static string GetHUDLabel(this WingmanOrder order)
    {
        switch (order)
        {
            case WingmanOrder.AttackLeaderTarget: return "ATTACK";
            case WingmanOrder.FollowLeader: return "FOLLOW";
            default: return "WAIT";
        }
    }
}

[RequireComponent(typeof(AITaskFollower))]
public class WingmanCaptain : MonoBehaviour
{
    [SerializeField]
    private WingmanOrder activeOrder;

    private AITaskFollower taskFollower;
    private AITask orderTask;

    struct PotentialTarget
    {
        public int Threat;
        public Targetable Target;
    }

    public Ship Ship
    {
        get { return taskFollower.Ship; }
    }

    public WingmanOrder ActiveOrder
    {
        get { return activeOrder; }
    }

    public void SetOrder(WingmanOrder newOrder)
    {
        if (activeOrder != newOrder && orderTask)
        {
            taskFollower.CancelTask(orderTask);
        }

        activeOrder = newOrder;
    }

    private void OnRadioMessage(RadioMessage message)
    {
        var fleet = Universe.FleetManager.GetFleetOf(Ship);

        if (message.SourceShip == fleet.Leader && fleet.Leader != Ship)
        {
            SetOrder((WingmanOrder) message.MessageType);

            //if we're the player's number two, acknowledge the order
            if (Universe.LocalPlayer && Universe.LocalPlayer.Ship == message.SourceShip)
            {
                var myFleet = Universe.FleetManager.GetFleetOf(Ship);
                if (myFleet && myFleet.Followers.IndexOf(Ship) == 0)
                {
                    StartCoroutine(AcknowledgeOrder(message));
                }
            }
        }

        if (fleet.Leader == Ship && message.MessageType == RadioMessageType.HelpMe)
        {
            //fleet member's in trouble, if we're not already attacking something else, send help

            //when asking for help you have to target the attacker!
            var attacker = message.SourceShip.Target;

            if (!Ship.Target)
            {
                Ship.Target = attacker;
                foreach (var follower in fleet.Followers)
                {
                    Ship.SendRadioMessage(RadioMessageType.Attack, follower);
                }
            }
        }
    }

    private IEnumerator AcknowledgeOrder(RadioMessage order)
    {
        yield return new WaitForSeconds(0.5f);
        Ship.SendRadioMessage(RadioMessageType.AcknowledgeOrder, order.SourceShip);
    }

    private int CalculateThreat(Targetable target)
    {
        int threat = 1;

        //invert threat for friendlies
        if (Ship.Targetable
            && Ship.Targetable.RelationshipTo(target) != TargetRelationship.Hostile)
        {
            threat = -threat;
        }

        const float COMFORT_ZONE = 20;

        var dist2 = (transform.position - target.transform.position).sqrMagnitude;
        if (dist2 < (COMFORT_ZONE * COMFORT_ZONE))
        {
            threat *= 2;
        }

        return threat;
    }

    private int AcquireTarget()
    {
        //todo: use saved local ships list
        var targetables = FindObjectsOfType(typeof(Targetable)) as Targetable[];
        if (targetables != null && targetables.Length > 0)
        {
            var potentialTargets = new List<PotentialTarget>(targetables.Length);

            foreach (var targetable in targetables)
            {
                if (targetable.gameObject == gameObject)
                {
                    continue;
                }

                potentialTargets.Add(new PotentialTarget()
                {
                    Target = targetable,
                    Threat = CalculateThreat(targetable)
                });
            }

            potentialTargets.RemoveAll(t => t.Threat < 0);

            if (potentialTargets.Count > 0)
            {
                //highest threat comes first
                potentialTargets.Sort((t1, t2) => t2.Threat - t1.Threat);

                Ship.Target = potentialTargets[0].Target;
                return potentialTargets[0].Threat;
            }
        }

        Ship.Target = null;
        return 0;
    }

	void Start()
	{
        taskFollower = GetComponent<AITaskFollower>();

        activeOrder = WingmanOrder.FollowLeader;
    }

    void OnTakeDamage(HitDamage damage)
    {
        var fm = Universe.FleetManager;
        var myFleet = fm.GetFleetOf(Ship);
        
        if (damage.Owner != null && 
            (!(myFleet && myFleet == fm.GetFleetOf(damage.Owner))))
        {
            //get angry and forget what we were doing if we're not already in attack mode
            if (activeOrder != WingmanOrder.AttackLeaderTarget
                && orderTask)
            {
                orderTask = null;
                taskFollower.ClearTasks();
            }

            //fight back
            if (damage.Owner.Targetable)
            {
                Ship.Target = damage.Owner.Targetable;

                if (!myFleet)
                {
                    //just go for it
                    taskFollower.QueueTask(AttackTask.Create(damage.Owner.Targetable));
                }
                else
                {
                    if (myFleet.Leader == Ship)
                    {
                        //sic the gang on them
                        foreach (var follower in myFleet.Followers)
                        {
                            Ship.SendRadioMessage(RadioMessageType.Attack, follower);
                        }
                    }
                    else
                    {
                        if (activeOrder != WingmanOrder.AttackLeaderTarget)
                        {
                            //ask leader for help
                            Ship.SendRadioMessage(RadioMessageType.HelpMe, myFleet.Leader);
                        }
                    }
                }
            }
        }
    }

	void Update()
    {
        var fleet = Universe.FleetManager.GetFleetOf(Ship);

        //check for targets myself
        AcquireTarget();

        if (!fleet)
        {
            SetOrder(WingmanOrder.Wait);
        }
        else if (fleet.Leader == Ship)
        {
            SetOrder(WingmanOrder.AttackLeaderTarget);
        }

        if (!orderTask)
        {
            switch (activeOrder)
            {
                case WingmanOrder.FollowLeader:
                    if (Ship.Target)
                    {
                        orderTask = AttackTask.Create(Ship.Target);
                    }
                    else
                    {
                        orderTask = FlyInFormationTask.Create(fleet.Leader);
                    }
                    break;
                case WingmanOrder.AttackLeaderTarget:
                    if (fleet.Leader.Target)
                    {
                        orderTask = AttackTask.Create(fleet.Leader.Target);
                    }
                    break;
                case WingmanOrder.Wait:
                    //while waiting, use individual target
                    if (Ship.Target)
                    {
                        orderTask = AttackTask.Create(Ship.Target);
                    }
                    break;
            }

            if (orderTask)
            {
                taskFollower.AssignTask(orderTask);
            }
        }
	}
}

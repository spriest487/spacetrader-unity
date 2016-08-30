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

[RequireComponent(typeof(AITaskFollower))]
public class WingmanCaptain : MonoBehaviour
{ 
    [SerializeField]
    private WingmanOrder activeOrder;

    private AITaskFollower taskFollower;
    private AITask orderTask;
    
    private float lastTargetCheck = 0;
    const float TARGET_CHECK_INTERVAL = 1;

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
	
	private Ship FindLeader()
	{
        var myFleet = SpaceTraderConfig.FleetManager.GetFleetOf(Ship);
        if (myFleet && myFleet.Leader != Ship)
        {
            return myFleet.Leader;
        }
        else
        {
            return null;
        }
	}

    private void SetOrder(WingmanOrder newOrder)
    {
        if (activeOrder != newOrder)
        {
            taskFollower.CancelTask(orderTask);

            activeOrder = newOrder;
        }
    }
    
    private void OnRadioMessage(RadioMessage message)
    {
        if (message.SourceShip == FindLeader())
        {
            SetOrder((WingmanOrder) message.MessageType);

            //if we're the player's number two, acknowledge the order
            if (PlayerShip.LocalPlayer && PlayerShip.LocalPlayer.Ship == message.SourceShip)
            {
                var myFleet = SpaceTraderConfig.FleetManager.GetFleetOf(Ship);
                if (myFleet && myFleet.Followers.IndexOf(Ship) == 0)
                {
                    StartCoroutine(AcknowledgeOrder(message));
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

    private void AcquireTarget()
    {
        var needsTarget = !Ship.Target
                || Time.time > (lastTargetCheck + TARGET_CHECK_INTERVAL);
        lastTargetCheck = Time.time;

        if (!needsTarget)
        {
            return;
        }

        //look for a target

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

            if (potentialTargets.Count == 0)
            {
                Ship.Target = null;
            }
            else
            {
                //highest threat comes first
                potentialTargets.Sort((t1, t2) => t2.Threat - t1.Threat);

                Ship.Target = potentialTargets[0].Target;
            }
        }
        else
        {
            Ship.Target = null;
        }
    }

	void Start()
	{
        taskFollower = GetComponent<AITaskFollower>();

        activeOrder = WingmanOrder.FollowLeader;
    }

	void Update()
    {
        var leader = FindLeader();

        if (!leader)
        {
            SetOrder(WingmanOrder.Wait);
        }

        if (!orderTask && leader)
        {
            switch (activeOrder)
            {
                case WingmanOrder.FollowLeader:
                    orderTask = FlyInFormationTask.Create(leader);
                    break;
                case WingmanOrder.AttackLeaderTarget:
                    if (leader.Target)
                    {
                        orderTask = AttackTask.Create(leader.Target);
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

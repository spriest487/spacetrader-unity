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
	
    private void SetOrder(WingmanOrder newOrder)
    {
        if (activeOrder != newOrder && orderTask)
        {
            taskFollower.CancelTask(orderTask);

            activeOrder = newOrder;
        }
    }
    
    private void OnRadioMessage(RadioMessage message)
    {
        var fleet = SpaceTraderConfig.FleetManager.GetFleetOf(Ship);

        if (message.SourceShip == fleet.Leader && fleet.Leader != Ship)
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
        var fm = SpaceTraderConfig.FleetManager;
        if (damage.Owner != null
            && fm.GetFleetOf(Ship) != fm.GetFleetOf(damage.Owner))
        {
            //get spooked and forget what we were doing if we're not attacking

            if (activeOrder != WingmanOrder.AttackLeaderTarget
                && orderTask)
            {
                taskFollower.CancelTask(orderTask);
                orderTask = null;
            }
        }
    }

	void Update()
    {
        var fleet = SpaceTraderConfig.FleetManager.GetFleetOf(Ship);

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

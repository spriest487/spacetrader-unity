#pragma warning disable 0649

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(Scanner))]
public class CombatAI : OrderableAI
{
    struct PotentialTarget
    {
        public int Threat;
        public Targetable Target;
    }
    
    public Scanner Scanner { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Scanner = GetComponent<Scanner>();
    }

    private void OnRadioMessage(RadioMessage message)
    {
        var fleet = Universe.FleetManager.GetFleetOf(Ship);

        if (message.SourceShip == fleet.Leader && fleet.Leader != Ship)
        {
            SetOrder((AIOrder) message.MessageType);

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

    private void OnTakeDamage(HitDamage damage)
    {
        var myFleet = Ship.GetFleet();
        Debug.Assert((myFleet && myFleet.Leader) || !myFleet);

        if (damage.Owner && 
            damage.Owner.Targetable &&
            (!myFleet || myFleet != damage.Owner.GetFleet()))
        {
            //fight back
            if (damage.Owner.Targetable)
            {
                //get angry and forget what we were doing if we're not already in attack mode
                if (ActiveOrder != AIOrder.Attack)
                {
                    Ship.Target = damage.Owner.Targetable;
                    SetOrder(AIOrder.Attack);
                }

                if (myFleet)
                {
                    if (myFleet.Leader == Ship)
                    {
                        /* i'm the boss, call the gang for help (tell them to attack my target) */
                        foreach (var follower in myFleet.Followers)
                        {
                            Ship.SendRadioMessage(RadioMessageType.Attack, follower);
                        }
                    }
                    else
                    {
                        /* i'm not the boss but i have a fleet, ask them for help! 
                         don't do this if we're already attacking, because we expect
                         to take various damage during combat */
                        if (ActiveOrder != AIOrder.Attack)
                        {
                            //ask leader for help
                            Ship.SendRadioMessage(RadioMessageType.HelpMe, myFleet.Leader);
                        }
                    }
                }
            }
        }
    }

	protected override void Update()
    {
        if (ActiveOrder == AIOrder.Wait)
        {
            var fleet = Ship.GetFleet();
            
            /* if in a fleet, first check to see if the boss has a target */
            if (fleet && fleet.Leader != this && fleet.Leader.Target)
            {
                Ship.Target = fleet.Leader.Target;
            }
            else
            {
                var highestThreat = Scanner.FindHighestThreat();
                Ship.Target = highestThreat.HasValue? highestThreat.Value.Targetable : null;
            }
            
            if (Ship.Target)
            {
                /* found something to attack - defer to the normal attack behaviour */
                SetOrder(AIOrder.Attack);
                base.Update();
            }
            else if (fleet && TaskFollower.Idle)
            {
                /* nothing to do, and i'm in a fleet, so just follow the boss */
                TaskFollower.AssignTask(FlyInFormationTask.Create(fleet));
            }
            else
            {
                base.Update();
            }
        }
        else
        {
            base.Update();
        }
	}
}

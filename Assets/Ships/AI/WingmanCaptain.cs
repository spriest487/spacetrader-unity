using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AITaskFollower))]
public class WingmanCaptain : MonoBehaviour
{    
	private const float FORMATION_SPACING = 1.5f;
	private const float FORMATION_MATCH_ANGLE = 15f;

	private Ship ship;
    private AICaptain captain;
    private AITaskFollower taskFollower;

    private Targetable targetable;

    private Vector3? immediateManeuver;
    
    const float MAX_PATH_AGE = 5f;

    private float lastTargetCheck = 0;
    const float TARGET_CHECK_INTERVAL = 1;
    
    struct PotentialTarget
    {
        public int Threat;
        public Targetable Target;
    }
	
	private Ship FindLeader()
	{
        if (PlayerShip.LocalPlayer
            && PlayerShip.LocalPlayer.GetComponent<Targetable>()
            && targetable
            && PlayerShip.LocalPlayer.GetComponent<Targetable>().Faction == targetable.Faction)
        {
            var leaderObj = PlayerShip.LocalPlayer;
            var leaderShip = leaderObj.GetComponent<Ship>();
            if (!leaderShip)
            {
                throw new UnityException("Wingman's Leader is not a Ship");
            }

            return leaderShip;
        }
        else
        {
            return null;
        }
	}

	private void FlyInFormation(Ship leader)
	{
		//fly in same direction as leader
		captain.Destination = transform.position + (leader.transform.forward * ship.BaseStats.maxSpeed);
		
		//keep adjusting to match our formation pos if possible
		captain.AdjustTarget = leader.GetFormationPos(ship);
		
		float angleDiffBetweenHeadings;		
		if (leader.GetComponent<Rigidbody>().velocity.sqrMagnitude > Vector3.kEpsilon)
		{
			angleDiffBetweenHeadings  = Mathf.Acos(Vector3.Dot(leader.GetComponent<Rigidbody>().velocity.normalized, leader.GetComponent<Rigidbody>().transform.forward));
		}
		else
		{
			angleDiffBetweenHeadings = 0;
		}
		
		angleDiffBetweenHeadings *= Mathf.Rad2Deg;

		if (angleDiffBetweenHeadings < FORMATION_MATCH_ANGLE)
		{
			//max speed is leader's speed
			captain.Throttle = Ship.EquivalentThrust(ship, leader);
		}
		else
		{
			//leader is going in a different direction to the one they are facing, wait and see what they do
			captain.Throttle = 0;
		}
	}

	private void CatchUpToLeader(Ship leader, float distance, float minFormationDistance)
	{
		var leaderPos = leader.transform.position;
		
		captain.Destination = leaderPos;

		//throttle down as we approach the min formation distance
		float throttleDownDist = ship.BaseStats.maxSpeed * Time.deltaTime;
		float remainingDist = Mathf.Max(0, distance - minFormationDistance);

		float catchupThrottle = Mathf.Min(1, remainingDist / throttleDownDist);

		//minimum speed is the leader's current speed
		//todo: take relative travel direction into account (facing away = full speed)		
		captain.Throttle = Mathf.Max(catchupThrottle, Ship.EquivalentThrust(ship, leader));
	}

    private void FollowLeader()
    {
        var leader = FindLeader();

        if (!leader)
        {
            return;
        }

        var myPos = GetComponent<Rigidbody>().transform.position;
        //var leaderBound = leader.rigidbody.ClosestPointOnBounds(transform.position);
        var leaderPos = leader.GetFormationPos(ship);

        //captain.adjustTarget = leaderPos;

        var distance = (myPos - leaderPos).magnitude;
        var minFormationDistance = GetComponent<Rigidbody>().GetComponent<Collider>().bounds.extents.magnitude * 1f;

        captain.TargetUp = null;
        captain.AdjustTarget = null;

        if (distance < minFormationDistance)
        {
            FlyInFormation(leader);
        }
        else
        {
            captain.Destination = leaderPos;
            captain.Throttle = 1;
        }
    }
    

    private void ChaseTarget()
    {
        //try to get on their six
        var TODO_CHASEDIST = 20;
        var behindTarget = ship.Target.transform.TransformPoint(new Vector3(0, 0, -TODO_CHASEDIST));
        
        var PANIC_DIST_FACTOR = 10;

        //if we get too close, panic for a little bit and fly away
        var between = transform.position - ship.Target.transform.position;
        if (between.sqrMagnitude < captain.CloseDistanceSqr * PANIC_DIST_FACTOR)
        {
            var panicVec = Random.onUnitSphere * captain.CloseDistance * PANIC_DIST_FACTOR;
            immediateManeuver = ship.Target.transform.position + panicVec;
        }

        for (int module = 0; module < ship.ModuleLoadout.HardpointModules.Count; ++module)
        {
            ship.ModuleLoadout.HardpointModules[module].Aim = ship.Target.transform.position;
            ship.ModuleLoadout.Activate(ship, module);
        }

        taskFollower.AssignTask(AttackTask.Create(ship.Target));
    }

    private int CalculateThreat(Targetable target)
    {
        int threat = 1;

        //invert threat for friendlies
        if (targetable && target.Faction == targetable.Faction)
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
        var needsTarget = !ship.Target
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
                ship.Target = null;
            }
            else
            {
                //highest threat comes first
                potentialTargets.Sort((t1, t2) => t2.Threat - t1.Threat);

                ship.Target = potentialTargets[0].Target;
            }
        }
        else
        {
            ship.Target = null;
        }
    }

    void FollowImmediateManeuver()
    {
        captain.Destination = immediateManeuver.Value;
        captain.Throttle = 1;

        if (captain.IsCloseTo(immediateManeuver.Value))
        {
            immediateManeuver = null;
        }
    }

	void Start()
	{
        taskFollower = GetComponent<AITaskFollower>();
        captain = taskFollower.Captain;

        ship = captain.Ship;

        targetable = GetComponent<Targetable>();
    }

	void Update()
    {
        if (immediateManeuver.HasValue)
        {
            FollowImmediateManeuver();
        }
        else
        {
            AcquireTarget();

            if (ship.Target)
            {
                ChaseTarget();
            }
            else
            {
                FollowLeader();
            }
        }
	}
}

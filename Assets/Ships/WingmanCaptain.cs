using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AICaptain), typeof(Ship))]
public class WingmanCaptain : MonoBehaviour
{
	private const float FORMATION_SPACING = 1.5f;
	private const float FORMATION_MATCH_ANGLE = 15f;

	private Ship ship;
	private AICaptain captain;
	
	private Ship FindLeader()
	{
		//TODO
        var leaderObj = PlayerShip.LocalPlayer;
		var leaderShip = leaderObj.GetComponent<Ship>();
		if (!leaderShip)
		{
			throw new UnityException("Wingman's Leader is not a Ship");
		}

		return leaderShip;
	}

	private void FlyInFormation(Ship leader)
	{
		//fly in same direction as leader
		captain.destination = transform.position + (leader.transform.forward * ship.Stats.maxSpeed);
		
		//keep adjusting to match our formation pos if possible
		captain.adjustTarget = leader.GetFormationPos(ship);
		
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

		//Debug.Log("Leader's velocity is " +leader.rigidbody.velocity);

		if (angleDiffBetweenHeadings < FORMATION_MATCH_ANGLE)
		{
			//max speed is leader's speed
			captain.throttle = Ship.EquivalentThrust(ship, leader);

			//Debug.Log("Flying alongside leader: angle is " +angleDiffBetweenHeadings);
		}
		else
		{
			//leader is going in a different direction to the one they are facing, wait and see what they do
			captain.throttle = 0;

			//Debug.Log("Stopping to match leader's orientation: angle is " + angleDiffBetweenHeadings);
		}

		/*Debug.Log(string.Format("Formation: leader's speed {0}, my speed/throttle {1}/{2}",
			leader.rigidbody.velocity,
			ship.rigidbody.velocity,
			captain.throttle));*/
	}

	private void CatchUpToLeader(Ship leader, float distance, float minFormationDistance)
	{
		var leaderPos = leader.transform.position;
		
		captain.destination = leaderPos;

		//throttle down as we approach the min formation distance
		float throttleDownDist = ship.Stats.maxSpeed * Time.deltaTime;
		float remainingDist = Mathf.Max(0, distance - minFormationDistance);

		float catchupThrottle = Mathf.Min(1, remainingDist / throttleDownDist);

		//minimum speed is the leader's current speed
		//todo: take relative travel direction into account (facing away = full speed)		
		captain.throttle = Mathf.Max(catchupThrottle, Ship.EquivalentThrust(ship, leader));
	}

	void Start()
	{
		ship = GetComponent<Ship>();
		captain = GetComponent<AICaptain>();
	}

	void Update() {
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

		captain.targetUp = null;
		captain.adjustTarget = null;

		if (distance < minFormationDistance)
		{
			FlyInFormation(leader);
		}
		else
		{
			captain.destination = leaderPos;
			captain.throttle = 1;
		}

		/*Debug.Log(string.Format("Min distance for formation flying is {0}, current distance {1}",
			minFormationDistance,
			distance));*/
	}
}

﻿using UnityEngine;
using Pathfinding;
using System.Collections;

[RequireComponent(typeof(Ship), typeof(Rigidbody))]
public class AICaptain : MonoBehaviour
{
	private const float AIM_ACCURACY = 2f;
    
	public Vector3 destination;

    /// <summary>
    /// if set we will try to rotate until our up vector matches this one.
    /// mostly for aesthetic purposes, ie flying in formation
    /// </summary>
	public Vector3? targetUp;

    /// <summary>
    /// Even if we're close to the destination, we'll use manual thruster control
    /// to hit this point exactly
    /// </summary>
	public Vector3? adjustTarget;

	public float Throttle;
    public float MinimumThrust;

    private Ship ship;

    //hiding the obsolete builtin Unity fields
    new private Rigidbody rigidbody;
    new private Collider collider;

    public Ship Ship
    {
        get
        {
            return ship;
        }
    }

    public float CloseDistance
    {
        get
        {
            return collider.bounds.extents.z * 2;
            //return ship.BaseStats.maxSpeed;
        }
    }

    public float CloseDistanceSqr
    {
        get
        {
            return CloseDistance * CloseDistance;
        }
    }

    private bool IsCloseTo(Vector3 point, Vector3 between)
    {
        return between.sqrMagnitude < CloseDistanceSqr;
    }

    public bool IsCloseTo(Vector3 point)
    {
        var between = point - transform.position;

        return IsCloseTo(point, between);
    }

    public bool CanSee(Collider target)
    {
        return false;
    }

    public bool CanSee(Vector3 target)
    {
        return false;
    }

    private void RotateTo(Vector3 between)
    {
        //direction vector towards dest, in local space
        var towards = between.normalized;
        var localTowards = transform.InverseTransformDirection(towards);

        Debug.DrawLine(transform.position, transform.position + (towards * 5), Color.cyan, Time.deltaTime);

        //local rotation required to get to target
        var rotateTo = Quaternion.LookRotation(localTowards, transform.up);

        var totalAngle = Vector3.Dot(towards, transform.forward);
        totalAngle = Mathf.Acos(totalAngle) * Mathf.Rad2Deg;
        
        var facingTowardsAngle = ship.BaseStats.maxTurnSpeed;
        var facingTowards = totalAngle < facingTowardsAngle;
        var facingDirectlyTowards = totalAngle < AIM_ACCURACY;

        var closeEnough = IsCloseTo(destination, between);

        var currentLocalRotation = transform.InverseTransformDirection(rigidbody.angularVelocity) * Mathf.Rad2Deg;

        if (!facingDirectlyTowards)
        {
            float turnFactor = Mathf.Clamp01(Mathf.Abs(totalAngle) / facingTowardsAngle);
            turnFactor = Mathf.Log10(1 + (turnFactor * 9)); //log10 slowdown instead of linear so we slow down more dramatically closer to the goal

            //Debug.Log(string.Format("turnFactor is {0:F4} (total angle is {1:F4}, slowdown angle is {2:F4})", turnFactor, totalAngle, facingTowardsAngle));

            //turn rotation into pitch/yaw/roll angles (with 90 being up, -90 being down, etc)
            var angles = rotateTo.eulerAngles;
            for (int angleIt = 0; angleIt < 3; ++angleIt)
            {
                var angle = angles[angleIt];
                angle = (angle > 180 ? -(360 - angle) : angle);

                var currentRotationThisAxis = currentLocalRotation[angleIt];

                if (MathUtils.SameSign(angle, currentRotationThisAxis) && Mathf.Abs(angle) < Mathf.Abs(currentRotationThisAxis))
                {
                    /*if we're already rotating in this direction faster than the
                     target speed, don't add any more thrust! */
                    angle = 0;
                }
                else
                {
                    if (angle > Vector3.kEpsilon)
                    {
                        angle = 1;
                    }
                    else if (angle < -Vector3.kEpsilon)
                    {
                        angle = -1;
                    }
                    else
                    {
                        angle = 0;
                    }
                }

                angle *= turnFactor;

                angles[angleIt] = angle;
            }

            ship.pitch = angles.x;
            ship.yaw = angles.y;
        }
        else
        {
            //within the "target zone" - try to counteract existing rotation to zero if possible
            Vector3 counterThrust = new Vector3();
            for (int a = 0; a < 3; ++a)
            {
                var angle = currentLocalRotation[a];
                counterThrust[a] = -(Mathf.Clamp01(angle / ship.BaseStats.maxTurnSpeed));
            }

            ship.pitch = counterThrust.x;
            ship.yaw = counterThrust.y;
            ship.roll = counterThrust.z;
        }

        if (!facingTowards)
        {
            //if not in danger, only thrust slowly when not facing target
            ship.thrust = 0.0f;
        }
        else
        {
            if (closeEnough)
            {
                if (facingDirectlyTowards)
                {
                    //if we know we're not rotating, and we're close to the target, use strafe and lift to adjust our pos towards the target
                    ship.thrust = 0;
                }
                else
                {
                    var distance = between.magnitude;

                    var desiredSpeed = Mathf.Clamp01(distance / CloseDistance);
                    var currentThrust = rigidbody.velocity.magnitude / ship.BaseStats.maxSpeed;

                    ship.thrust = currentThrust > desiredSpeed ? -1 : desiredSpeed;
                }
            }
            else
            {
                ship.thrust = 1;
            }
        }

        ship.thrust = Mathf.Max(MinimumThrust, ship.thrust);
    }

    void Start()
    {
        ship = GetComponent<Ship>();
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }
    					
	void FixedUpdate () {        
		Debug.DrawLine(transform.position, destination, Color.red, Time.deltaTime);

        if (adjustTarget.HasValue)
        {
            Debug.DrawLine(transform.position, adjustTarget.Value, Color.magenta, Time.deltaTime);
        }
        
		var between = destination - transform.position;
        if (between.sqrMagnitude > Vector3.kEpsilon)
        {
            RotateTo(between);
        }

		ship.strafe = Mathf.Clamp(ship.strafe, -1, Throttle);
		ship.lift = Mathf.Clamp(ship.lift, -1, Throttle);
		ship.thrust = Mathf.Clamp(ship.thrust, -1, Throttle);

		//adjustments are applied AFTER the clamping, so we can go beyond the throttle if necessary
		ApplyAdjustThrust(ship);
	}

	private void ApplyAdjustThrust(Ship ship) 
	{
		var currentLocalSpeed = transform.InverseTransformDirection(rigidbody.velocity);

		var maxAdjust = 1 - Mathf.Abs(ship.thrust);
		if (adjustTarget.HasValue && maxAdjust > Vector3.kEpsilon)
		{
			/* if we have an adjustment target, let's use any remaining thrust to move toward it */

			var adjustBetween = adjustTarget.Value - ship.transform.position;

			float adjustPower = Mathf.Clamp01(adjustBetween.sqrMagnitude);
			adjustPower = Mathf.Log10(1 + (adjustPower * 9)); //log10 slowdown instead of linear so we slow down more dramatically closer to the goal

			adjustPower *= maxAdjust;
            
			var localBetween = ship.transform.InverseTransformDirection(adjustBetween);
            
			float betweenDimMax = Mathf.Max(
				Mathf.Abs(localBetween.x),
				Mathf.Abs(localBetween.y),
				Mathf.Abs(localBetween.z));

			Vector3 newThrust = new Vector3(ship.strafe, ship.lift, ship.thrust);
			if (betweenDimMax > Vector3.kEpsilon)
			{
				for (var i = 0; i < 3; ++i)
				{
					/* if we're already moving in this direction, but we're going faster
					 * than we're actually trying to thrust, let's assume we're going too
					 fast and fire the opposite thruster to slow down a bit */
					float thrustToCounteract = currentLocalSpeed[i];

					if (MathUtils.SameSign(thrustToCounteract, localBetween[i]) && Mathf.Abs(thrustToCounteract) > adjustPower)
					{
						newThrust[i] = -thrustToCounteract;
					}
					else
					{
						float adjustThrust = (localBetween[i] / betweenDimMax);

						newThrust[i] = adjustThrust * adjustPower;
					}
				}
			}

			ship.strafe = newThrust.x;
			ship.lift = newThrust.y;
			ship.thrust = newThrust.z;
		}
        else
        {
            //we don't use these if we're not adjusting
            ship.strafe = 0;
            ship.lift = 0;
        }
	}
}

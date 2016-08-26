#pragma warning disable 0649

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Ship), typeof(Rigidbody))]
public class AICaptain : MonoBehaviour
{
	private const float AIM_ACCURACY = 2f;
    
	public Vector3? Destination;

    /// <summary>
    /// if set we will try to rotate until our up vector matches this one.
    /// mostly for aesthetic purposes, ie flying in formation
    /// </summary>
	public Vector3? TargetUp;

    /// <summary>
    /// Even if we're close to the destination, we'll use manual thruster control
    /// to hit this point exactly
    /// </summary>
	public Vector3? AdjustTarget;

	public float Throttle;
    public float MinimumThrust;

    private Ship ship;

    //hiding the obsolete builtin Unity fields
    new private Rigidbody rigidbody;

    public Ship Ship
    {
        get
        {
            return ship;
        }
    }

    void Start()
    {
        ship = GetComponent<Ship>();
        rigidbody = GetComponent<Rigidbody>();

        Destination = null;
        TargetUp = null;
    }
    					
	void Update()
    {        
        if (!Destination.HasValue)
        {
            return;
        }

		Debug.DrawLine(transform.position, Destination.Value, Color.red, Time.deltaTime);

        if (AdjustTarget.HasValue)
        {
            Debug.DrawLine(transform.position, AdjustTarget.Value, Color.magenta, Time.deltaTime);
        }
        
		var between = Destination.Value - transform.position;
        if (between.sqrMagnitude > Vector3.kEpsilon)
        {
            ship.RotateToDirection(Destination.Value, TargetUp, AIM_ACCURACY);
        }

		ship.Strafe = Mathf.Clamp(ship.Strafe, -1, Throttle);
		ship.Lift = Mathf.Clamp(ship.Lift, -1, Throttle);
		ship.Thrust = Mathf.Clamp(ship.Thrust, -1, Throttle);

        //if we're not facing where we want to go, slow down so we can turn
        var shipToDest = (Destination.Value - transform.position).normalized;
        var forwardToDestDot = Vector3.Dot(shipToDest, transform.forward);
        var slowDownToTurnFactor = (forwardToDestDot + 1) * 0.5f;
        ship.Thrust = Mathf.Clamp(ship.Thrust, MinimumThrust, slowDownToTurnFactor);

        AdjustThrottleForProximity(between);

        if (ship.IsCloseTo(Destination.Value))
        {
            //adjustments are applied AFTER the clamping, so we can go beyond the throttle if necessary
            ApplyAdjustThrust();
        }
	}

    private void AdjustThrottleForProximity(Vector3 between)
    {
        var dist = between.magnitude;

        //TODO: could calculate stopping time exactly, this is assuming 1 second
        var slowdownDist = ship.CloseDistance + ship.CurrentStats.MaxSpeed;

        var slowdownFactor = Mathf.Clamp01(dist / slowdownDist);

        ship.Thrust *= slowdownFactor;
    }
    
	private void ApplyAdjustThrust() 
	{
		var currentLocalSpeed = transform.InverseTransformDirection(rigidbody.velocity);

		var maxAdjust = 1 - Mathf.Abs(ship.Thrust);
		if (AdjustTarget.HasValue && maxAdjust > Vector3.kEpsilon)
		{
			/* if we have an adjustment target, let's use any remaining thrust to move toward it */

			var adjustBetween = AdjustTarget.Value - ship.transform.position;

			float adjustPower = Mathf.Clamp01(adjustBetween.sqrMagnitude);
			adjustPower = Mathf.Log10(1 + (adjustPower * 9)); //log10 slowdown instead of linear so we slow down more dramatically closer to the goal

			adjustPower *= maxAdjust;
            
			var localBetween = ship.transform.InverseTransformDirection(adjustBetween);
            
			float betweenDimMax = Mathf.Max(
				Mathf.Abs(localBetween.x),
				Mathf.Abs(localBetween.y),
				Mathf.Abs(localBetween.z));

			Vector3 newThrust = new Vector3(ship.Strafe, ship.Lift, ship.Thrust);
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

			ship.Strafe = newThrust.x;
			ship.Lift = newThrust.y;
			ship.Thrust = newThrust.z;
		}
        else
        {
            //we don't use these if we're not adjusting
            ship.Strafe = 0;
            ship.Lift = 0;
        }
	}

    private void OnRadioMessage(RadioMessage message)
    {
        if (message.SourceShip != Ship)
        {
            if (message.MessageType == RadioMessageType.Greeting)
            {
                //reply!
                StartCoroutine(WaitThenReply(message.SourceShip));
            }
        }
    }

    private IEnumerator WaitThenReply(Ship source)
    {
        yield return new WaitForSeconds(1);

        Ship.SendRadioMessage(RadioMessageType.Greeting, source);
    }
}

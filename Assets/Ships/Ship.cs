using UnityEngine;
using System.Collections.Generic;

public class Ship : MonoBehaviour
{
	public FormationManager formationManager;

    public SpaceStation dockedIn;

    public ShipStats stats;

    public Targetable target;
	
	public float thrust;
	public float strafe;
	public float lift;

	public float pitch;
	public float yaw;
	public float roll;

	public Vector3 aim;

	/**
	 * Finds the equivalent thrust required for the "from" ship to match
	 * the current speed of the "target" ship (value will not exceed 1, even
	 * if "from" is unable to match the speed)
	 */
	public static float EquivalentThrust(Ship from, Ship target)
	{
		var targetSpeed = target.rigidbody.velocity.magnitude;
		var maxSpeed = Mathf.Max(1, from.stats.maxSpeed);
		var result = Mathf.Clamp01(targetSpeed / maxSpeed);
		
		return result;
	}

	void Start()
	{
		formationManager = new FormationManager();
	}

    private static Vector3 InputAmountsToRequired(Vector3 input,
        Vector3 localCurrentValue,
        float maxSpeed)
    {
        var inputAdjusted = input;
        for (int i = 0; i < 3; ++i)
        {
            var localAmt = localCurrentValue[i] / maxSpeed;
            var inputAmt = input[i];
            var sign = Mathf.Sign(inputAmt);

            var localAmtAbs = Mathf.Abs(localAmt);
            var inputAmtAbs = Mathf.Abs(inputAmt);

            if (inputAmtAbs > Mathf.Epsilon)
            {
                inputAdjusted[i] = (inputAmtAbs - localAmtAbs) * sign;
            }
        }

        return inputAdjusted;
    }

	private static Vector3 DecayRotation(Vector3 input,
		Vector3 localCurrentValue,
		float decaySpeed,
		float maxSpeed,
		Transform currentTransform)
	{
         var inputAdjusted = input;

		//input proportional to the largest individual value
		var inputMax = Mathf.Max(Mathf.Abs(inputAdjusted.x), Mathf.Abs(inputAdjusted.y), Mathf.Abs(inputAdjusted.z));
		Vector3 inputProportions;

		if (inputMax > Vector3.kEpsilon)
		{
			inputProportions = new Vector3(inputAdjusted.x / inputMax, inputAdjusted.y / inputMax, inputAdjusted.z / inputMax);
		}
		else
		{
			inputProportions = Vector3.zero;
		}

		var decayAmt = Time.deltaTime * decaySpeed;
		var decay = new Vector3(decayAmt, decayAmt, decayAmt);
		
		/* input can counteract decay, so the direction we're actually trying to go is not
		 * decayed. 100% input will counteract an amount of decay equal to 100% of decaySpeed,
		 * 50% input will counteract 50%, etc
		 */
		for (int i = 0; i < 3; ++i)
		{
			decay[i] *= 1 - (inputAdjusted[i] * inputProportions[i]);
		}
		
		if (decay.sqrMagnitude <= Vector3.kEpsilon)
		{
			decay = Vector3.zero;
		}

        var decayedRot = localCurrentValue;
		for (int i = 0; i < 3; ++i) 
		{
            var absRot = Mathf.Abs(localCurrentValue[i]);

			if (absRot > Vector3.kEpsilon)
			{
				/* we're decaying an absolute amount, but how we apply it is by multiplying current
				 velocity, so we need the decay speed as proportion of current velocity in this
				 direction */				
				var decayProportion = Mathf.Clamp01(decay[i] / absRot);

				decayedRot[i] *= (1 - (decayProportion));
			}
		}

        return currentTransform.TransformDirection(decayedRot);
	}
			
	void FixedUpdate()
	{			
		formationManager.Update();
		DebugDrawFollowerPositions();

		if (rigidbody)
		{
            rigidbody.drag = 0;
            rigidbody.angularDrag = 0;
            rigidbody.maxAngularVelocity = Mathf.Deg2Rad * stats.maxTurnSpeed;
            rigidbody.inertiaTensor = new Vector3(1, 1, 1);

            //all movement vals must be within -1..1
            thrust = Mathf.Clamp(thrust, -1, 1);
            strafe = Mathf.Clamp(strafe, -1, 1);
            lift = Mathf.Clamp(lift, -1, 1);
            pitch = Mathf.Clamp(pitch, -1, 1);
            yaw = Mathf.Clamp(yaw, -1, 1);
            roll = Mathf.Clamp(roll, -1, 1);

            var torqueMax = stats.maxTurnSpeed * Mathf.Deg2Rad;

            var localRotation = rigidbody.transform.InverseTransformDirection(rigidbody.angularVelocity);
            var localVelocity = rigidbody.transform.InverseTransformDirection(rigidbody.velocity);

            var torqueInput = InputAmountsToRequired(new Vector3(pitch, yaw, roll),
                localRotation,
                torqueMax);            
            var forceInput = InputAmountsToRequired(new Vector3(strafe, lift, thrust),
                localVelocity,
                stats.maxSpeed);           

            rigidbody.angularVelocity = DecayRotation(torqueInput,
                localRotation,
                stats.agility * Mathf.Deg2Rad,
                torqueMax,
                rigidbody.transform);
            rigidbody.velocity = DecayRotation(forceInput,
                localVelocity,
                stats.thrust,
                stats.maxSpeed,
                rigidbody.transform);

            var force = forceInput.normalized * stats.thrust;
            var torque = torqueInput.normalized * Mathf.Deg2Rad * stats.agility;

            /* apply new forces */
            rigidbody.AddRelativeTorque(torque);
            rigidbody.AddRelativeForce(force);
		}
	}

	void LateUpdate()
	{
		//limit speed
		//float maxCurrentSpeed = stats.maxSpeed - rigidbody.drag;
		float speed = rigidbody.velocity.magnitude;
		if (speed > stats.maxSpeed)
		{
			rigidbody.velocity = rigidbody.velocity.normalized * stats.maxSpeed;
		}
	}
	
	void OnCollisionEnter(Collision collision)
	{
		Debug.Log(string.Format("Ship collided with {0} with speed {1} (magnitude {2})", collision.collider.gameObject, collision.relativeVelocity, collision.relativeVelocity.magnitude));
	}
	
	private Vector3 GetFormationPos(int followerId)
	{
		var shipPos = rigidbody.transform.position;

		var posIndex = formationManager.IncludeFollower(followerId);
		if (posIndex != 0)
		{
			var spacing = rigidbody.collider.bounds.extents.magnitude * 4;
			var offset = rigidbody.transform.right * posIndex * spacing;

			return shipPos + offset;
		}
		else
		{
			return shipPos;
		}
	}

	public Vector3 GetFormationPos(Ship follower)
	{
		return GetFormationPos(follower.GetInstanceID());
	}

	private void DebugDrawFollowerPositions()
	{
		foreach (var follower in formationManager.followers)
		{
			var pos = GetFormationPos(follower);

			var debugOff = (rigidbody.transform.forward * rigidbody.collider.bounds.extents.magnitude * 0.5f);

			Debug.DrawLine(pos - debugOff, pos + debugOff, Color.green, Time.deltaTime);
			//Debug.Log("pos: " +pos);
		}
	}
}

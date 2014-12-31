using UnityEngine;
using System.Collections.Generic;

public class Ship : MonoBehaviour
{
	public ShipStats _stats;
	public Hitpoints _hitpoints;
	public bool _targettable = true;

	public FormationManager formationManager; 
	
	public ShipStats stats { get { return _stats; } }
	public bool targettable { get { return _targettable; } }
	public Hitpoints hitpoints { get { return _hitpoints;  } }
	
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
		var maxSpeed = from.stats.maxSpeed;
		var result = Mathf.Clamp01(targetSpeed / maxSpeed);
		
		return result;
	}

	void Start()
	{
		formationManager = new FormationManager();
		_hitpoints = null;
	}

	private static Vector3 DecayRotation(Vector3 input,
		Vector3 currentValue,
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

		//convert to pitch, yaw, roll
		var localRot = currentTransform.InverseTransformDirection(currentValue);

		var decayAmt = Time.deltaTime * decaySpeed;
		var decay = new Vector3(decayAmt, decayAmt, decayAmt);
		
		/* input can counteract decay, so the direction we're actually trying to go is not
		 * decayed. 100% input will counteract an amount of decay equal to 100% of decaySpeed,
		 * 50% input will counteract 50%, etc
		 */
		for (int i = 0; i < 3; ++i)
		{
			decay[i] *= 1 - (inputAdjusted[i] * inputProportions[i]);
			//decay[i] = Mathf.Max(0, decay[i]);

			/* if the speed in this direction is greater than the input though, double the decay! */
			if (MathUtils.SameSign(localRot[i], inputAdjusted[i]))
			{
				//var currentSpeed = Mathf.Abs(localRot[i]) / maxSpeed;
				//var pastTarget = currentSpeed - Mathf.Abs(input[i]);

				//if (pastTarget > 0)
				//{
				//	//Debug.Log(currentSpeed.ToString("F4"));

				//	decay[i] = decayAmt * (1 + pastTarget);
				//}
			}
		}
		
		if (decay.sqrMagnitude <= Vector3.kEpsilon)
		{
			decay = Vector3.zero;
		}

		var decayedRot = localRot;
		for (int i = 0; i < 3; ++i) 
		{
			var absRot = Mathf.Abs(localRot[i]);

			if (absRot > Vector3.kEpsilon)
			{
				/* we're decaying an absolute amount, but how we apply it is by multiplying current
				 velocity, so we need the decay speed as proportion of current velocity in this
				 direction */				
				var decayProportion = Mathf.Clamp01(decay[i] / absRot);

				decayedRot[i] *= (1 - (decayProportion));
			}
		}

		//Debug.Log(string.Format("{0} => {1} (decay: {2})", localRot.ToString("F3"), decayedRot.ToString("F3"), decay));

		localRot = decayedRot;

		return currentTransform.TransformDirection(localRot);
	}
			
	void FixedUpdate()
	{
		rigidbody.drag = 0;
		rigidbody.angularDrag = 0;
		rigidbody.maxAngularVelocity = Mathf.Deg2Rad * stats.maxTurnSpeed;
		rigidbody.inertiaTensor = new Vector3(1, 1, 1);
		
		formationManager.Update();
		DebugDrawFollowerPositions();

		if (!rigidbody)
		{
			return;
		}

		//all movement vals must be within -1..1
		thrust = Mathf.Clamp(thrust, -1, 1);
		strafe = Mathf.Clamp(strafe, -1, 1);
		lift = Mathf.Clamp(lift, -1, 1);
		pitch = Mathf.Clamp(pitch, -1, 1);
		yaw = Mathf.Clamp(yaw, -1, 1);
		roll = Mathf.Clamp(roll, -1, 1);

		var forceInput = new Vector3(strafe, lift, thrust);
		var torqueInput = new Vector3(pitch, yaw, roll);

		rigidbody.angularVelocity = DecayRotation(torqueInput,
			rigidbody.angularVelocity,
			stats.agility * Mathf.Deg2Rad,
			stats.maxTurnSpeed * Mathf.Deg2Rad,
			rigidbody.transform);
		rigidbody.velocity = DecayRotation(forceInput,
			rigidbody.velocity,
			stats.thrust,
			stats.maxSpeed,
			rigidbody.transform);		
		
		var force = forceInput.normalized * stats.thrust;
		var torque = torqueInput.normalized * Mathf.Deg2Rad * stats.agility;

		/* apply new forces */
		rigidbody.AddRelativeTorque(torque);
		rigidbody.AddRelativeForce(force);
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

	void OnCollisionEnter(Collision collision)
	{
		Debug.Log(string.Format("Ship collided with {0} with speed {1} (magnitude {2})", collision.collider.gameObject, collision.relativeVelocity, collision.relativeVelocity.magnitude));
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

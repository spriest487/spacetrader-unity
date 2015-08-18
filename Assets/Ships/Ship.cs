using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class Ship : MonoBehaviour
{
    [SerializeField]
	private FormationManager formationManager;

    [SerializeField]
    private ShipStats stats;

    [SerializeField]
    private Targetable target;
    
    [SerializeField]
    private ScalableParticle explosionEffect;
	
	public float thrust;
	public float strafe;
	public float lift;

	public float pitch;
	public float yaw;
	public float roll;

	public Vector3 aim;
    
    public ShipStats Stats { get { return stats; } set {
        stats = new ShipStats(value);
    }}
    public Targetable Target { get { return target; } set { target = value; } }

    public ScalableParticle ExplosionEffect
    {
        get { return explosionEffect; }
        set { explosionEffect = value; }
    }

    private Vector3? bumpForce;

	/**
	 * Finds the equivalent thrust required for the "from" ship to match
	 * the current speed of the "target" ship (value will not exceed 1, even
	 * if "from" is unable to match the speed)
	 */
	public static float EquivalentThrust(Ship from, Ship target)
	{
		var targetSpeed = target.GetComponent<Rigidbody>().velocity.magnitude;
		var maxSpeed = Mathf.Max(1, from.stats.maxSpeed);
		var result = Mathf.Clamp01(targetSpeed / maxSpeed);
		
		return result;
	}

	void Awake()
	{
        //default these members in for instances not created in the editor
        if (formationManager == null)
        {
            formationManager = new FormationManager();
        }

        if (stats == null)
        {
            stats = new ShipStats();
        }
	}

    private void ApplyBump(Rigidbody rigidbody)
    {
        if (!bumpForce.HasValue)
        {
            return;
        }

        float bumpMag2 = bumpForce.Value.sqrMagnitude;
        float bumpReduction = stats.maxSpeed * Time.deltaTime;
        bumpReduction *= bumpReduction;

        float reducedBumpMag = Mathf.Max(0, bumpMag2 - bumpReduction);
        float reductionFactor = reducedBumpMag / bumpMag2;

        bumpForce = bumpForce.Value * reductionFactor;

        rigidbody.AddForce(bumpForce.Value);

        if (bumpForce.Value.sqrMagnitude < bumpReduction)
        {
            bumpForce = null;
        }
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

        var result = currentTransform.TransformDirection(decayedRot);

        for (int component = 0; component < 3; ++component)
        {
            if (float.IsNaN(result[component]))
            {
                result[component] = 0;
            }
        }

        return result;       
	}
			
	void FixedUpdate()
	{			
		formationManager.Update();
		DebugDrawFollowerPositions();

        var rigidBody = GetComponent<Rigidbody>();
        if (rigidBody)
		{
            rigidBody.drag = 0;
            rigidBody.angularDrag = 0;
            rigidBody.maxAngularVelocity = Mathf.Deg2Rad * stats.maxTurnSpeed;
            rigidBody.inertiaTensor = new Vector3(1, 1, 1);

            //all movement vals must be within -1..1
            thrust = Mathf.Clamp(thrust, -1, 1);
            strafe = Mathf.Clamp(strafe, -1, 1);
            lift = Mathf.Clamp(lift, -1, 1);
            pitch = Mathf.Clamp(pitch, -1, 1);
            yaw = Mathf.Clamp(yaw, -1, 1);
            roll = Mathf.Clamp(roll, -1, 1);

            var torqueMax = stats.maxTurnSpeed * Mathf.Deg2Rad;

            var localRotation = rigidBody.transform.InverseTransformDirection(rigidBody.angularVelocity);
            var localVelocity = rigidBody.transform.InverseTransformDirection(rigidBody.velocity);

            var torqueInput = InputAmountsToRequired(new Vector3(pitch, yaw, roll),
                localRotation,
                torqueMax);            
            var forceInput = InputAmountsToRequired(new Vector3(strafe, lift, thrust),
                localVelocity,
                stats.maxSpeed);           

            rigidBody.angularVelocity = DecayRotation(torqueInput,
                localRotation,
                stats.agility * Mathf.Deg2Rad,
                torqueMax,
                rigidBody.transform);
            rigidBody.velocity = DecayRotation(forceInput,
                localVelocity,
                stats.thrust,
                stats.maxSpeed,
                rigidBody.transform);

            var force = forceInput.normalized * stats.thrust;
            var torque = torqueInput.normalized * Mathf.Deg2Rad * stats.agility;

            /* apply new forces */
            rigidBody.AddRelativeTorque(torque);
            rigidBody.AddRelativeForce(force);

            ApplyBump(rigidBody);
		}
	}

	void LateUpdate()
	{
		//limit speed
		//float maxCurrentSpeed = stats.maxSpeed - rigidbody.drag;
        var rigidBody = GetComponent<Rigidbody>();
        
        float speed = rigidBody.velocity.magnitude;
        if (speed > stats.maxSpeed)
        {
            rigidBody.velocity = rigidBody.velocity.normalized * stats.maxSpeed;
        }
	}
	
	void OnCollisionStay(Collision collision)
	{
		Debug.Log(string.Format("Ship collided with {0} with speed {1} (magnitude {2})", collision.collider.gameObject, collision.relativeVelocity, collision.relativeVelocity.magnitude));

        //a little bump in the opposite direction
        var rigidbody = GetComponent<Rigidbody>();
        if (rigidbody)
        {
            var bumpPower = collision.relativeVelocity;

            bumpPower *= -1;

            this.bumpForce = bumpPower;
        }
	}
	
	private Vector3 GetFormationPos(int followerId)
	{
		var shipPos = GetComponent<Rigidbody>().transform.position;

		var posIndex = formationManager.IncludeFollower(followerId);
		if (posIndex != 0)
		{
			var spacing = GetComponent<Rigidbody>().GetComponent<Collider>().bounds.extents.magnitude * 4;
			var offset = GetComponent<Rigidbody>().transform.right * posIndex * spacing;

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

			var debugOff = (GetComponent<Rigidbody>().transform.forward * GetComponent<Rigidbody>().GetComponent<Collider>().bounds.extents.magnitude * 0.5f);

			Debug.DrawLine(pos - debugOff, pos + debugOff, Color.green, Time.deltaTime);
			//Debug.Log("pos: " +pos);
		}
	}

    void OnTakeDamage(HitDamage hd)
    {
        var hp = GetComponent<Hitpoints>();
        if (hp && hp.GetArmor() - hd.Amount <= 0)
        {
            if (explosionEffect)
            {
                var explosion = (ScalableParticle) Instantiate(explosionEffect, transform.position, transform.rotation);
                explosion.ParticleScale = 0.5f;
            }

            Destroy(gameObject);
        }
    }
}

#pragma warning disable 0649

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class Ship : MonoBehaviour
{
    [SerializeField]
	private FormationManager formationManager = new FormationManager();

    [SerializeField]
    private ShipCrewAssignments crewAssignments = new ShipCrewAssignments();

    [SerializeField]
    private ShipStats baseStats;

    [SerializeField]
    private Targetable target;
    
    [SerializeField]
    private ScalableParticle explosionEffect;

    [SerializeField]
    private List<Ability> abilities = new List<Ability>();

    [SerializeField]
    private List<StatusEffect> activeStatusEffects = new List<StatusEffect>();

    [SerializeField]
    private Vector3 bumpForce;

    [SerializeField]
    private CargoHold cargo;

    [SerializeField]
    private ModuleLoadout moduleLoadout;

    private List<WeaponHardpoint> hardpoints;
    
    public float Thrust;
    public float Strafe;
    public float Lift;
    public float Pitch;
    public float Yaw;
    public float Roll;
    
    private ShipStats currentStats;

    public IList<WeaponHardpoint> Hardpoints
    {
        get
        {
            if (hardpoints == null)
            {
                hardpoints = new List<WeaponHardpoint>(GetComponentsInChildren<WeaponHardpoint>());

                if (hardpoints.Count == 0)
                {
                    var defaultHardpoint = gameObject.AddComponent<WeaponHardpoint>();
                    defaultHardpoint.Arc = 360;
                    hardpoints.Add(defaultHardpoint);
                }
            }

            return hardpoints;
        }
    }
    
    public ShipStats CurrentStats
    {
        get
        {
            if (currentStats == null)
            {
                RecalculateCurrentStats();
            }
            return currentStats;
        }
    }

    public ModuleLoadout ModuleLoadout
    {
        get { return moduleLoadout; }
    }
    
    public ShipStats BaseStats
    {
        get { return baseStats; }
        set { baseStats = new ShipStats(value); }
    }

    public Targetable Target
    {
        get { return target; }
        set
        {
#if UNITY_EDITOR
            if (PlayerShip.LocalPlayer 
                && PlayerShip.LocalPlayer.Ship == this)
            {
                if (value)
                {
                    UnityEditor.Selection.activeGameObject = value.gameObject;
                }
                else
                {
                    UnityEditor.Selection.activeGameObject = null;
                }
            }
#endif
            target = value;
        }
    }

    public CargoHold Cargo
    {
        get { return cargo; }
        set { cargo = value; }
    }

    public IList<Ability> Abilities
    {
        get { return abilities.AsReadOnly(); }
        set { abilities = new List<Ability>(value); }
    }

    public ScalableParticle ExplosionEffect
    {
        get { return explosionEffect; }
        set { explosionEffect = value; }
    }

    public IList<StatusEffect> StatusEffects
    {
        get { return activeStatusEffects.AsReadOnly(); }
    }

    public ShipCrewAssignments CrewAssignments
    {
        get { return crewAssignments; }
    }
   
    /**
	 * Finds the equivalent thrust required for the "from" ship to match
	 * the current speed of the "target" ship (value will not exceed 1, even
	 * if "from" is unable to match the speed)
	 */
    public static float EquivalentThrust(Ship from, Ship target)
	{
		var targetSpeed = target.GetComponent<Rigidbody>().velocity.magnitude;
		var maxSpeed = Mathf.Max(1, from.CurrentStats.maxSpeed);
		var result = Mathf.Clamp01(targetSpeed / maxSpeed);
		
		return result;
	}

    private void OnDestroy()
    {
        foreach (var effect in activeStatusEffects)
        {
            Destroy(effect);
        }

        foreach (var ability in abilities)
        {
            Destroy(ability);
        }

        var fleetManager = SpaceTraderConfig.FleetManager;
        if (fleetManager)
        {
            SpaceTraderConfig.FleetManager.LeaveFleet(this);
        }
    }

    private void OnDisable()
    {
        target = null;
    }

    private void RecalculateCurrentStats()
    {
        var result = new ShipStats(BaseStats);

        ShipStats proportionalTotals = new ShipStats();

        foreach (var statusEffect in activeStatusEffects)
        {
            result.AddFlat(statusEffect.FlatStats);
            proportionalTotals.AddFlat(statusEffect.ProportionalStats);
        }

        result.ApplyProportional(proportionalTotals);

        currentStats = result;
    }

    public void AddStatusEffect(StatusEffect effect)
    {
        if (effect == null)
        {
            Debug.LogErrorFormat("tried to add null status effect to {0}", name);
        }

        if (RemoveStatusEffect(effect))
        {
            Debug.LogWarningFormat("removed status effect {0} from {1} because it was added again", effect.name, name);
        }

        activeStatusEffects.Add(effect);
    }

    public bool RemoveStatusEffect(StatusEffect effect)
    {
        return activeStatusEffects.Remove(effect);        
    }
    
    void Awake()
	{
        //default these members in for instances not created in the editor
        if (formationManager == null)
        {
            formationManager = new FormationManager();
        }

        if (baseStats == null)
        {
            baseStats = new ShipStats();
        }

        if (moduleLoadout == null)
        {
            moduleLoadout = new ModuleLoadout();
        }

        activeStatusEffects.RemoveAll(e => e == null);
	}

    private void ApplyBump(Rigidbody rigidbody)
    {
        if (bumpForce.sqrMagnitude < Mathf.Epsilon)
        {
            return;
        }

        float bumpMag2 = bumpForce.sqrMagnitude;
        float bumpReduction = CurrentStats.maxSpeed * Time.deltaTime;
        bumpReduction *= bumpReduction;

        float reducedBumpMag = Mathf.Max(0, bumpMag2 - bumpReduction);

        if (reducedBumpMag > 0 && bumpMag2 > 0)
        {
            float reductionFactor = reducedBumpMag / bumpMag2;

            bumpForce = bumpForce * reductionFactor;

            rigidbody.AddForce(bumpForce);

            if (bumpForce.sqrMagnitude < bumpReduction)
            {
                bumpForce = Vector3.zero;
            }
        }
        else
        {
            bumpForce = Vector3.zero;
        }
    }

    private static Vector3 InputAmountsToRequired(Vector3 input,
        Vector3 localCurrentValue,
        float maxSpeed)
    {
        if (maxSpeed < float.Epsilon)
        {
            return Vector3.zero;
        }

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
            rigidBody.maxAngularVelocity = Mathf.Deg2Rad * CurrentStats.maxTurnSpeed;
            rigidBody.inertiaTensor = new Vector3(1, 1, 1);

            //all movement vals must be within -1..1
            Thrust = Mathf.Clamp(Thrust, -1, 1);
            Strafe = Mathf.Clamp(Strafe, -1, 1);
            Lift = Mathf.Clamp(Lift, -1, 1);
            Pitch = Mathf.Clamp(Pitch, -1, 1);
            Yaw = Mathf.Clamp(Yaw, -1, 1);
            Roll = Mathf.Clamp(Roll, -1, 1);

            var torqueMax = CurrentStats.maxTurnSpeed * Mathf.Deg2Rad;

            var localRotation = rigidBody.transform.InverseTransformDirection(rigidBody.angularVelocity);
            var localVelocity = rigidBody.transform.InverseTransformDirection(rigidBody.velocity);

            var torqueInput = InputAmountsToRequired(new Vector3(Pitch, Yaw, Roll),
                localRotation,
                torqueMax);            
            var forceInput = InputAmountsToRequired(new Vector3(Strafe, Lift, Thrust),
                localVelocity,
                CurrentStats.maxSpeed);           

            rigidBody.angularVelocity = DecayRotation(torqueInput,
                localRotation,
                CurrentStats.agility * Mathf.Deg2Rad,
                torqueMax,
                rigidBody.transform);
            rigidBody.velocity = DecayRotation(forceInput,
                localVelocity,
                CurrentStats.thrust,
                CurrentStats.maxSpeed,
                rigidBody.transform);

            var force = forceInput.normalized * CurrentStats.thrust;
            var torque = torqueInput.normalized * Mathf.Deg2Rad * CurrentStats.agility;

            /* apply new forces */
            rigidBody.AddRelativeTorque(torque);
            rigidBody.AddRelativeForce(force);

            ApplyBump(rigidBody);
		}
	}

	private void Update()
	{
        //force refresh of stats
        currentStats = null;

        //ability cooldowns
        foreach (var ability in abilities)
        {
            if (ability.Cooldown > 0)
            {
                ability.Cooldown -= Time.deltaTime;
            }
        }

        //modules
        ModuleLoadout.Update();

        List<StatusEffect> newStatusEffects = new List<StatusEffect>(activeStatusEffects.Count);
        foreach (var effect in activeStatusEffects)
        {
            effect.Lifetime -= Time.deltaTime;
            if (!effect.Expires || effect.Lifetime > 0)
            {
                newStatusEffects.Add(effect);
            }
            else 
            {
                Destroy(effect);
            }
        }
        activeStatusEffects = newStatusEffects;

		//limit speed
		//float maxCurrentSpeed = stats.maxSpeed - rigidbody.drag;
        var rigidBody = GetComponent<Rigidbody>();
        
        float speed = rigidBody.velocity.magnitude;
        if (speed > CurrentStats.maxSpeed)
        {
            rigidBody.velocity = rigidBody.velocity.normalized * CurrentStats.maxSpeed;
        }
	}
	
	void OnCollisionStay(Collision collision)
	{
		//a little bump in the opposite direction
        var rigidbody = GetComponent<Rigidbody>();
        if (rigidbody)
        {
            var bumpPower = collision.relativeVelocity;

            bumpPower *= -1;

            bumpForce = bumpPower;
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

    public WeaponHardpoint GetHardpointAt(int index)
    {
        return Hardpoints[index % Hardpoints.Count];
    }

    public void Explode()
    {
        if (explosionEffect)
        {
            var explosion = (ScalableParticle)Instantiate(explosionEffect, transform.position, transform.rotation);
            explosion.ParticleScale = 0.5f;
        }

        Destroy(gameObject);
    }

    void OnTakeDamage(HitDamage hd)
    {
        var hp = GetComponent<Hitpoints>();
        if (hp && hp.GetArmor() - hd.Amount <= 0)
        {
            Explode();
        }
    }

    public void SendRadioMessage(RadioMessageType message, Ship target)
    {
        var messageTargets = new List<Ship>();
        if (target)
        {
            //private, just me and the target
            messageTargets.Add(target);
            messageTargets.Add(this);
        }
        else
        {
            //all ships everywhere
            messageTargets.AddRange(FindObjectsOfType<Ship>());
        }

        var radioMessage = new RadioMessage(this, message);        
        foreach (var messageTarget in messageTargets)
        {
            messageTarget.SendMessage("OnRadioMessage", radioMessage, SendMessageOptions.DontRequireReceiver);
        }
    }

    public bool IsFleetMember(Ship other)
    {
        var fleet = SpaceTraderConfig.FleetManager.GetFleetOf(this);

        return fleet && fleet.IsMember(other);
    }
}

#pragma warning disable 0649

using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class Ship : MonoBehaviour
{
    [SerializeField]
	private FormationManager formationManager = new FormationManager();
        
    [SerializeField]
    private Targetable target;
    
    [SerializeField]
    private ScalableParticle explosionEffect;

    [SerializeField]
    private List<Ability> abilities = new List<Ability>();

    [SerializeField]
    private List<StatusEffect> activeStatusEffects = new List<StatusEffect>();
    
    [SerializeField]
    private CargoHold cargo;

    [SerializeField]
    private ModuleLoadout moduleLoadout;

    private new Rigidbody rigidbody;

    private List<WeaponHardpoint> hardpoints;
    
    public float Thrust;
    public float Strafe;
    public float Lift;
    public float Pitch;
    public float Yaw;
    public float Roll;
    
    private ShipStats currentStats;

    [SerializeField]
    private ShipType shipType;

    public ShipType ShipType
    {
        get { return shipType; }
    }

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

    public IEnumerable<StatusEffect> StatusEffects
    {
        get { return activeStatusEffects.AsReadOnly(); }
    }
    
    public CrewMember GetCaptain()
    {
        return SpaceTraderConfig.CrewConfiguration.Characters
            .Where(c => c.AssignedShip == this && c.AssignedRole == CrewAssignment.Captain)
            .FirstOrDefault();
    }

    public IEnumerable<CrewMember> GetPassengers()
    {
        return SpaceTraderConfig.CrewConfiguration.Characters
            .Where(c => c.AssignedShip == this && c.AssignedRole == CrewAssignment.Passenger)
            .ToList();
    }

    public static Ship Create(GameObject obj, ShipType shipType)
    {
        var ship = obj.AddComponent<Ship>();
        ship.shipType = shipType;

        var rb = ship.GetComponent<Rigidbody>();
        UpdateRigidBodyFromStats(rb, shipType.Stats);

        ship.ExplosionEffect = shipType.ExplosionEffect;

        var newAbilities = new List<Ability>();
        foreach (var ability in shipType.Abilities)
        {
            if (ability != null)
            {
                var abilityInstance = Instantiate(ability);
                abilityInstance.name = ability.name;
                abilityInstance.Cooldown = 0;
                newAbilities.Add(abilityInstance);
            }
        }
        ship.Abilities = newAbilities;

        if (shipType.Moorable && !obj.gameObject.GetComponent<Moorable>())
        {
            obj.gameObject.AddComponent<Moorable>();
        }

        if (!ship.cargo)
        {
            ship.Cargo = ScriptableObject.CreateInstance<CargoHold>();
        }
        ship.Cargo.Size = shipType.CargoSize;

        var hp = obj.gameObject.AddComponent<Hitpoints>();
        hp.Reset(shipType.Stats.Armor, shipType.Stats.Shield);

        ship.moduleLoadout.SlotCount = shipType.ModuleSlots;

        return ship;
    }

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private static void UpdateRigidBodyFromStats(Rigidbody rigidbody, ShipStats stats)
    {
        rigidbody.mass = stats.Mass;
        rigidbody.isKinematic = rigidbody.mass < Mathf.Epsilon;

        rigidbody.drag = 2;
        rigidbody.angularDrag = 2;
        rigidbody.maxAngularVelocity = Mathf.Deg2Rad * stats.maxTurnSpeed;
        rigidbody.inertiaTensor = new Vector3(1, 1, 1);
    }
   
    /**
	 * Finds the equivalent thrust required for the "from" ship to match
	 * the current speed of the "target" ship (value will not exceed 1, even
	 * if "from" is unable to match the speed)
	 */
    public static float EquivalentThrust(Ship from, Ship target)
	{
		var targetSpeed = target.rigidbody.velocity.magnitude;
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

    public void RecalculateCurrentStats()
    {
        var result = ShipType.Stats.Clone();
        var proportionalTotals = new ShipStats();

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
        
        if (moduleLoadout == null)
        {
            moduleLoadout = new ModuleLoadout();
        }

        activeStatusEffects.RemoveAll(e => e == null);
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
    
	void FixedUpdate()
	{
		formationManager.Update();
		DebugDrawFollowerPositions();
        
        if (rigidbody)
		{
            UpdateRigidBodyFromStats(rigidbody, CurrentStats);

            //all movement vals must be within -1..1
            Thrust = Mathf.Clamp(Thrust, -1, 1);
            Strafe = Mathf.Clamp(Strafe, -1, 1);
            Lift = Mathf.Clamp(Lift, -1, 1);
            Pitch = Mathf.Clamp(Pitch, -1, 1);
            Yaw = Mathf.Clamp(Yaw, -1, 1);
            Roll = Mathf.Clamp(Roll, -1, 1);

            var torqueMax = CurrentStats.maxTurnSpeed * Mathf.Deg2Rad;

            var localRotation = transform.InverseTransformDirection(rigidbody.angularVelocity);
            var localVelocity = transform.InverseTransformDirection(rigidbody.velocity);

            var torqueInput = InputAmountsToRequired(new Vector3(Pitch, Yaw, Roll),
                localRotation,
                torqueMax);            
            var forceInput = InputAmountsToRequired(new Vector3(Strafe, Lift, Thrust),
                localVelocity,
                CurrentStats.maxSpeed);
            
            var force = forceInput.normalized * CurrentStats.thrust;
            var torque = torqueInput.normalized * Mathf.Deg2Rad * CurrentStats.agility;

            /* apply new forces */
            rigidbody.AddRelativeTorque(torque);
            rigidbody.AddRelativeForce(force);
		}
	}

	private void Update()
	{
        if (!ShipType)
        {
            Debug.LogError("missing shiptype for ship object " + gameObject.name);
            gameObject.SetActive(false);
        }

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

        //module ticks
        foreach (var module in moduleLoadout)
        {
            module.UpdateBehaviour(this);
        }

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

    public float EstimateDps()
    {
        return moduleLoadout.Where(mod => mod.ModuleType.Behaviour is IWeapon)
            .Select(mod => ((IWeapon)mod.ModuleType.Behaviour).CalculateDps(this))
            .Sum();
    }
}

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

    [Header("Controls")]

    [SerializeField]
    private float thrust;

    [SerializeField]
    private float strafe;

    [SerializeField]
    private float lift;

    [SerializeField]
    private float pitch;

    [SerializeField]
    private float yaw;

    [SerializeField]
    private float roll;
    
    private new Collider collider;

    private Hitpoints hitPoints;

    private List<WeaponHardpoint> hardpoints;
    
    public float Thrust
    {
        get { return thrust; }
        set { thrust = value; }
    }

    public float Strafe
    {
        get { return strafe; }
        set { strafe = value; }
    }

    public float Lift
    {
        get { return lift; }
        set { lift = value; }
    }

    public float Pitch
    {
        get { return pitch; }
        set { pitch = value; }
    }

    public float Yaw
    {
        get { return yaw; }
        set { yaw = value; }
    }

    public float Roll
    {
        get { return roll; }
        set { roll = value; }
    }

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
                UpdateHardpoints();
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

    public Rigidbody RigidBody { get; private set; }
    public Targetable Targetable { get; private set; }
    public Moorable Moorable { get; private set; }

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

    public float CloseDistance
    {
        get
        {
            var extents = collider.bounds.extents;
            return Mathf.Max(extents.x, extents.y, extents.z) * 4;
        }
    }

    public float CloseDistanceSqr
    {
        get
        {
            var closeDistance = CloseDistance;
            return closeDistance * closeDistance;
        }
    }

    public void ResetControls(float pitch = 0, float yaw = 0, float roll = 0, float thrust = 0, float strafe = 0, float lift = 0)
    {
        Pitch = pitch;
        Yaw = yaw;
        Roll = roll;
        Thrust = thrust;
        Strafe = strafe;
        Lift = lift;
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

    public IEnumerable<CrewMember> GetAllCrew()
    {
        var captain = GetCaptain();
        if (captain)
        {
            yield return GetCaptain();
        }
        
        foreach (var passenger in GetPassengers())
        {
            yield return passenger;
        }
    }

    private void UpdateHardpoints()
    {
        hardpoints = new List<WeaponHardpoint>(GetComponentsInChildren<WeaponHardpoint>());

        if (hardpoints.Count == 0)
        {
            var defaultHardpoint = gameObject.AddComponent<WeaponHardpoint>();
            defaultHardpoint.Arc = 360;
            hardpoints.Add(defaultHardpoint);
        }
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

        ship.hitPoints = obj.gameObject.AddComponent<Hitpoints>();
        ship.hitPoints.Reset(shipType.Stats.Armor, shipType.Stats.Shield);

        ship.moduleLoadout.SlotCount = shipType.ModuleSlots;

        ship.UpdateHardpoints();

        return ship;
    }

    void Start()
    {
        RigidBody = GetComponent<Rigidbody>();

        //ships are targetable by default 
        Targetable = GetComponent<Targetable>();
        if (!Targetable)
        {
            Targetable = gameObject.AddComponent<Targetable>();
        }

        collider = GetComponent<Collider>();
        Moorable = GetComponent<Moorable>();
    }

    private static void UpdateRigidBodyFromStats(Rigidbody rigidbody, ShipStats stats)
    {
        rigidbody.mass = stats.Mass;
        rigidbody.isKinematic = rigidbody.mass < Mathf.Epsilon;

        rigidbody.drag = 2;
        rigidbody.angularDrag = 2;
        rigidbody.maxAngularVelocity = Mathf.Deg2Rad * stats.MaxTurnSpeed;
        rigidbody.inertiaTensor = new Vector3(1, 1, 1);
    }
   
    /**
	 * Finds the equivalent thrust required for the "from" ship to match
	 * the current speed of the "target" ship (value will not exceed 1, even
	 * if "from" is unable to match the speed)
	 */
    public static float EquivalentThrust(Ship from, Ship target)
	{
		var targetSpeed = target.RigidBody.velocity.magnitude;
		var maxSpeed = Mathf.Max(1, from.CurrentStats.MaxSpeed);
		var result = Mathf.Clamp01(targetSpeed / maxSpeed);
		
		return result;
	}

    private bool IsCloseTo(Vector3 point, Vector3 between, float distance)
    {
        return between.sqrMagnitude < CloseDistanceSqr;
    }

    public bool IsCloseTo(Vector3 point, float distance)
    {
        var between = point - transform.position;

        return IsCloseTo(point, between, distance);
    }

    public bool IsCloseTo(Vector3 point)
    {
        return IsCloseTo(point, CloseDistance);
    }

    public bool CanSee(Vector3 target)
    {
        /* the between ray is backwards from the target, since raycasts ignore colliders
        that the origin point is inside */
        var between = transform.position - target;
        var ray = new Ray(target, between);

        foreach (var hit in Physics.RaycastAll(ray, between.magnitude))
        {
            if (hit.collider != collider)
            {
                return false;
            }
        }
        return true;
    }

    public void RotateToPoint(Vector3 aimPoint, Vector3? targetUp = null, float aimAccuracy = 1)
    {
        var between = aimPoint - transform.position;

        if (between.sqrMagnitude < Mathf.Epsilon)
        {
            return;
        }

        RotateToDirection(between.normalized, targetUp, aimAccuracy);
    }

    public void RotateToDirection(Vector3 aimDir, Vector3? targetUp = null, float aimAccuracy = 1)
    {
        var aimDirLocal = transform.InverseTransformDirection(aimDir);

        Debug.DrawLine(transform.position, transform.position + (aimDir * 5), Color.cyan, Time.deltaTime);

        //local rotation required to get to target
        var rotateTo = Quaternion.LookRotation(aimDirLocal, targetUp.HasValue ? targetUp.Value : transform.up);

        var totalAngle = Mathf.Clamp(Vector3.Dot(aimDir, transform.forward), -1, 1);
        totalAngle = Mathf.Acos(totalAngle) * Mathf.Rad2Deg;

        var facingTowardsAngle = Mathf.Max(1, CurrentStats.MaxTurnSpeed);
        var facingDirectlyTowards = totalAngle < aimAccuracy;

        //var closeEnough = IsCloseTo(Destination.Value, between, CloseDistance);

        var currentLocalRotation = transform.InverseTransformDirection(RigidBody.angularVelocity) * Mathf.Rad2Deg;

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

            Pitch = angles.x;
            Yaw = angles.y;
        }
        else
        {
            //within the "target zone" - try to counteract existing rotation to zero if possible
            Vector3 counterThrust = new Vector3();
            for (int a = 0; a < 3; ++a)
            {
                var angle = currentLocalRotation[a];
                counterThrust[a] = -(Mathf.Clamp01(angle / Mathf.Max(1, CurrentStats.MaxTurnSpeed)));
            }

            Pitch = counterThrust.x;
            Yaw = counterThrust.y;
            Roll = counterThrust.z;
        }

        //if (!facingTowards)
        //{
        //    //if not in danger, only thrust slowly when not facing target
        //    Thrust = 0.0f;
        //}
        //else
        //{
        //    if (closeEnough)
        //    {
        //        if (facingDirectlyTowards)
        //        {
        //            //if we know we're not rotating, and we're close to the target, use strafe and lift to adjust our pos towards the target
        //            Thrust = 0;
        //        }
        //        else
        //        {
        //            var distance = between.magnitude;

        //            var desiredSpeed = Mathf.Clamp01(distance / CloseDistance);
        //            var currentThrust = rigidbody.velocity.magnitude / CurrentStats.MaxSpeed;

        //            Thrust = currentThrust > desiredSpeed ? -1 : desiredSpeed;
        //        }
        //    }
        //    else
        //    {
        //        Thrust = 1;
        //    }
        //}
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

        var captain = GetCaptain();
        if (captain)
        {
            var skillStats = new ShipStats()
            {
                Agility = 0.05f * captain.PilotSkill,
                Thrust = 0.05f * captain.PilotSkill,
                ArmorRaw = 0.05f * captain.MechanicalSkill,
                ShieldRaw = 0.05f * captain.MechanicalSkill,
                DamageMultiplier = 0.05f * captain.WeaponsSkill
            };

            result.ApplyProportional(skillStats);
        }

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
            var localAmt = maxSpeed == 0f? 0f : localCurrentValue[i] / maxSpeed;
            var inputAmt = input[i];
            var sign = Mathf.Sign(inputAmt);

            var localAmtAbs = Mathf.Abs(localAmt);
            var inputAmtAbs = Mathf.Abs(inputAmt);

            if (inputAmtAbs > Mathf.Epsilon)
            {
                inputAdjusted[i] = (inputAmtAbs - localAmtAbs) * sign;
            }
        }

        if (inputAdjusted.sqrMagnitude > 1)
        {
            inputAdjusted.Normalize();
        }

        return inputAdjusted;
    }
    
	void FixedUpdate()
	{
		formationManager.Update();
		DebugDrawFollowerPositions();
        
        if (RigidBody)
		{
            UpdateRigidBodyFromStats(RigidBody, CurrentStats);

            //all movement vals must be within -1..1
            Thrust = Mathf.Clamp(Thrust, -1, 1);
            Strafe = Mathf.Clamp(Strafe, -1, 1);
            Lift = Mathf.Clamp(Lift, -1, 1);
            Pitch = Mathf.Clamp(Pitch, -1, 1);
            Yaw = Mathf.Clamp(Yaw, -1, 1);
            Roll = Mathf.Clamp(Roll, -1, 1);

            var torqueMax = CurrentStats.MaxTurnSpeed * Mathf.Deg2Rad;

            var localRotation = transform.InverseTransformDirection(RigidBody.angularVelocity);
            var localVelocity = transform.InverseTransformDirection(RigidBody.velocity);

            var torqueInput = InputAmountsToRequired(new Vector3(Pitch, Yaw, Roll),
                localRotation,
                torqueMax);
            var forceInput = InputAmountsToRequired(new Vector3(Strafe, Lift, Thrust),
                localVelocity,
                CurrentStats.MaxSpeed);
                                    
            var force = forceInput * CurrentStats.Thrust;
            var torque = torqueInput * Mathf.Deg2Rad * CurrentStats.Agility;

            for (int i = 0; i <3; ++i)
            {
                if (float.IsNaN(torque[i]))
                    Debug.Log("test");
            }

            /* apply new forces */
            RigidBody.AddRelativeTorque(torque);
            RigidBody.AddRelativeForce(force);
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
                
        float speed = RigidBody.velocity.magnitude;
        if (speed > CurrentStats.MaxSpeed)
        {
            RigidBody.velocity = RigidBody.velocity.normalized * CurrentStats.MaxSpeed;
        }
	}
		
	private Vector3 GetFormationPos(int followerId)
	{
        var shipPos = transform.position;

		var posIndex = formationManager.IncludeFollower(followerId);
		if (posIndex != 0)
		{
			var spacing = collider.bounds.extents.magnitude * 4;
			var offset = RigidBody.transform.right * posIndex * spacing;

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
   
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void DebugDrawFollowerPositions()
	{
		foreach (var follower in formationManager.followers)
		{
			var pos = GetFormationPos(follower);

			var debugOff = (RigidBody.transform.forward * collider.bounds.extents.magnitude * 0.5f);

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
        //multiple hits can happen in the same frame, and destroy us
        //check we're still alive
        if (!isActiveAndEnabled)
        {
            return;
        }

        if (hitPoints && hitPoints.GetArmor() - hd.Amount <= 0)
        {
            if (hd.Owner)
            {
                var xp = CalculateXPReward();

                hd.Owner.GrantCrewXP(xp);
            }

            SpaceTraderConfig.QuestBoard.NotifyDeath(this, hd.Owner);

            //:(
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

    public void GrantCrewXP(int amount)
    {
        var crewMembers = GetAllCrew().ToList();
        if (crewMembers.Count == 0)
        {
            return;
        }

        var perMember = Mathf.FloorToInt((float)amount / crewMembers.Count);
        for (int member = 0; member < crewMembers.Count; ++member)
        {
            crewMembers[member].GrantXP(perMember);
        }
    }

    public float EstimateDps()
    {
        return moduleLoadout.Select(mod => mod.Behaviour? mod.Behaviour.CalculateDps(this) : 0)
            .Sum();
    }

    public int ApplyDamageModifier(int baseDamage)
    {
        var mod = currentStats.DamageMultiplier;

        mod = Mathf.Max(1f, mod);

        return Mathf.FloorToInt(baseDamage * mod);
    }

    public int CalculateXPReward()
    {
        var baseXp = shipType.XPReward;

        var captain = GetCaptain();

        return baseXp + Mathf.FloorToInt(baseXp * 0.5f * (captain? captain.Level : 0));
    }
}

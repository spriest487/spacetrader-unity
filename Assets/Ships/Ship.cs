#pragma warning disable 0649

using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public partial class Ship : MonoBehaviour
{
    public const string SHIP_TAG = "Ship";

    [SerializeField]
    private Targetable target;

    [HideInInspector]
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

    private Hitpoints hitPoints;

    private List<WeaponHardpoint> hardpoints;

    public float Thrust
    {
        get { return thrust; }
    }

    public float Strafe
    {
        get { return strafe; }
    }

    public float Lift
    {
        get { return lift; }
    }

    public float Pitch
    {
        get { return pitch; }
    }

    public float Yaw
    {
        get { return yaw; }
    }

    public float Roll
    {
        get { return roll; }
    }

    private ShipStats currentStats;

    [SerializeField]
    private ShipType shipType;

    public ShipType ShipType
    {
        get { return shipType; }
    }

    public IEnumerable<WeaponHardpoint> Hardpoints
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
            if (Universe.LocalPlayer
                && Universe.LocalPlayer.Ship == this)
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
    public Collider Collider { get; private set; }
    public Targetable Targetable { get; private set; }
    public DockableObject Dockable { get; private set; }

    public IEnumerable<Ability> Abilities
    {
        get { return abilities.AsReadOnly(); }
        set { abilities = new List<Ability>(value); }
    }

    public IEnumerable<StatusEffect> StatusEffects
    {
        get { return activeStatusEffects.AsReadOnly(); }
    }

    public float CloseDistance
    {
        get
        {
            var extents = Collider.bounds.extents;
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
    
    public Ability GetAbility(int ability)
    {
        return abilities[ability];
    }

    public void ResetControls(float pitch = 0, float yaw = 0, float roll = 0, float thrust = 0, float strafe = 0, float lift = 0)
    {
        Debug.Assert(!float.IsNaN(pitch));
        Debug.Assert(!float.IsNaN(yaw));
        Debug.Assert(!float.IsNaN(roll));
        Debug.Assert(!float.IsNaN(thrust));
        Debug.Assert(!float.IsNaN(strafe));
        Debug.Assert(!float.IsNaN(lift));

        this.pitch = pitch;
        this.yaw = yaw;
        this.roll = roll;
        this.thrust = thrust;
        this.strafe = strafe;
        this.lift = lift;
    }

    public CrewMember GetCaptain()
    {
        return Universe.CrewConfiguration.Characters
            .Where(c => c.AssignedShip == this && c.AssignedRole == CrewAssignment.Captain)
            .FirstOrDefault();
    }

    public IEnumerable<CrewMember> GetPassengers()
    {
        return Universe.CrewConfiguration.Characters
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

        ship.RigidBody = ship.GetComponent<Rigidbody>();
        UpdateRigidBodyFromStats(ship.RigidBody, shipType.Stats);

        ship.Abilities = shipType.Abilities.Where(a => !!a)
            .Select(a =>
            {
                var abilityInstance = Instantiate(a);
                abilityInstance.name = a.name;
                abilityInstance.Cooldown = 0;
                return abilityInstance;
            });

        if (shipType.Dockable && !obj.gameObject.GetComponent<DockableObject>())
        {
            ship.Dockable = obj.gameObject.AddComponent<DockableObject>();
        }

        if (!ship.cargo)
        {
            ship.Cargo = ScriptableObject.CreateInstance<CargoHold>();
        }
        ship.Cargo.Size = shipType.CargoSize;

        Debug.Assert(!ship.GetComponent<Hitpoints>(), "ship base instance should not already have a Hitpoints component");

        ship.hitPoints = obj.gameObject.AddComponent<Hitpoints>();
        ship.hitPoints.Reset(shipType.Stats.Armor, shipType.Stats.Shield);

        ship.moduleLoadout = new ModuleLoadout { SlotCount = shipType.ModuleSlots };

        ship.UpdateHardpoints();

        if (shipType.Targetable)
        {
            Debug.Assert(!ship.GetComponent<Targetable>() && !ship.Targetable);
            ship.Targetable = ship.gameObject.AddComponent<Targetable>();
        }

        ship.Collider = ship.GetComponentInChildren<Collider>();

        /* we nearly always need this... */
        AITaskFollower.AddToShip(ship);

        return ship;
    }

    void Awake()
    {
        gameObject.tag = SHIP_TAG;

        RigidBody = GetComponent<Rigidbody>();

        Targetable = GetComponent<Targetable>();

        hitPoints = GetComponent<Hitpoints>();
        Collider = GetComponent<Collider>();
        Dockable = GetComponent<DockableObject>();

#if UNITY_EDITOR
        Debug.Assert(!abilities
            .Where(a => UnityEditor.PrefabUtility.GetPrefabType(a) != UnityEditor.PrefabType.None)
            .Any(),
            "on spawn, no ship should reference ability assets directly, they should be cloned!");
#endif

        if (moduleLoadout == null)
        {
            moduleLoadout = new ModuleLoadout();
        }

        activeStatusEffects.RemoveAll(e => e == null);
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
            if (hit.collider != Collider)
            {
                return false;
            }
        }
        return true;
    }

    public bool RotateToPoint(Vector3 aimPoint, Vector3? targetUp = null, float aimAccuracy = 1)
    {
        var between = aimPoint - transform.position;

        if (between.sqrMagnitude < Mathf.Epsilon)
        {
            return true;
        }

        return RotateToDirection(between.normalized, targetUp, aimAccuracy);
    }

    public bool RotateToDirection(Vector3 aimDir, Vector3? targetUp = null, float aimAccuracy = 1)
    {
        var aimDirLocal = transform.InverseTransformDirection(aimDir);

        Debug.DrawLine(transform.position, transform.position + (aimDir * 5), Color.cyan, Time.deltaTime);

        //local rotation required to get to target
        var rotateTo = Quaternion.LookRotation(aimDirLocal, targetUp ?? transform.up);

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

            pitch = angles.x;
            yaw = angles.y;
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

            pitch = counterThrust.x;
            yaw = counterThrust.y;
            roll = counterThrust.z;
        }

        float safeTurnAngle = 30;

        //slow down our forwards thrust to turn, if we're going too fast
        var dotToTarget = Mathf.Clamp(Vector3.Dot(transform.forward, aimDir), -1.0f, 1.0f);
        float degToTarget = Mathf.Rad2Deg * Mathf.Acos(dotToTarget);
        float brakeToTurnFactor = Mathf.Clamp01(1 - (degToTarget / safeTurnAngle));

        thrust *= brakeToTurnFactor;
        lift *= brakeToTurnFactor;
        strafe *= brakeToTurnFactor;

        Debug.Assert(!float.IsNaN(pitch));
        Debug.Assert(!float.IsNaN(yaw));
        Debug.Assert(!float.IsNaN(roll));
        Debug.Assert(!float.IsNaN(thrust));
        Debug.Assert(!float.IsNaN(strafe));
        Debug.Assert(!float.IsNaN(lift));

        return facingDirectlyTowards;
    }

    public void PreciseManeuverTo(Vector3 moveToPos)
    {
        var adjustBetween = moveToPos - transform.position;
        var localBetween = transform.InverseTransformDirection(adjustBetween);

        float adjustMax = Mathf.Max(
            Mathf.Abs(localBetween.x),
            Mathf.Abs(localBetween.y),
            Mathf.Abs(localBetween.z));

        Vector3 newThrust;
        if (Mathf.Approximately(0, adjustMax))
        {
            newThrust = Vector3.zero;
        }
        else
        {
            newThrust = localBetween / adjustMax;

            //adjust less forcefully if we're close to the destination
            var safeAdjustDist = CurrentStats.MaxSpeed * 2; //TODO calculate stopping distance?
            
            if (!Mathf.Approximately(0, safeAdjustDist))
            {
                float adjustPower = Mathf.Clamp01(localBetween.magnitude / safeAdjustDist);

                newThrust *= adjustPower;
            }
        }

        strafe = newThrust.x;
        lift = newThrust.y;

        //don't intefere with thrust if we're going faster anyway
        if (Mathf.Abs(newThrust.z) > Mathf.Abs(thrust))
        {
            thrust = newThrust.z;
        }

        Debug.Assert(!float.IsNaN(thrust));
        Debug.Assert(!float.IsNaN(strafe));
        Debug.Assert(!float.IsNaN(lift));
    }
    private void OnDestroy()
    {
        foreach (var ability in abilities)
        {
            Destroy(ability);
        }

        var fleetManager = Universe.FleetManager;
        if (fleetManager)
        {
            Universe.FleetManager.LeaveFleet(this);
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
        if (RemoveStatusEffect(effect))
        {
            Debug.LogWarningFormat("removed status effect {0} from {1} because it was added again", effect.Name, name);
        }

        activeStatusEffects.Add(effect);
    }

    public bool RemoveStatusEffect(StatusEffect effect)
    {
        return activeStatusEffects.Remove(effect);
    }

	void FixedUpdate()
	{
        if (RigidBody)
		{
            UpdateRigidBodyFromStats(RigidBody, CurrentStats);

            //all movement vals must be within -1..1
            thrust = Mathf.Clamp(thrust, -1, 1);
            strafe = Mathf.Clamp(strafe, -1, 1);
            lift = Mathf.Clamp(lift, -1, 1);
            pitch = Mathf.Clamp(pitch, -1, 1);
            yaw = Mathf.Clamp(yaw, -1, 1);
            roll = Mathf.Clamp(roll, -1, 1);

            var torqueInput = new Vector3(Pitch, Yaw, Roll);
            var forceInput = new Vector3(Strafe, Lift, Thrust);

            var force = forceInput * CurrentStats.Thrust;
            var torque = torqueInput * Mathf.Deg2Rad * CurrentStats.Agility;

            for (int i = 0; i <3; ++i)
            {
                Debug.Assert(!float.IsNaN(torque[i]));
                Debug.Assert(!float.IsNaN(force[i]));
            }

            /* apply new forces */
            RigidBody.AddRelativeTorque(torque);
            RigidBody.AddRelativeForce(force);
		}
	}

    public void ActivateWeapons()
    {
        if (Target)
        {
            for (int mod = 0; mod < ModuleLoadout.SlotCount; ++mod)
            {
                var module = ModuleLoadout.GetSlot(mod);
                module.Aim = Target.transform.position;
                module.Activate(this, mod);
            }
        }
    }

	private void Update()
	{
        Debug.Assert(ShipType, "ship " + name + " must have a shiptype");
        
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

        activeStatusEffects.RemoveAll(effect => effect.Expires <= Time.time);

        float speed = RigidBody.velocity.magnitude;
        if (speed > CurrentStats.MaxSpeed)
        {
            RigidBody.velocity = RigidBody.velocity.normalized * CurrentStats.MaxSpeed;
        }
	}
    
    public WeaponHardpoint GetHardpointAt(int index)
    {
        return hardpoints[index % hardpoints.Count];
    }

    public void Explode()
    {
        if (shipType && shipType.ExplosionEffect)
        {
            var explosion = (ScalableParticle)Instantiate(shipType.ExplosionEffect, transform.position, transform.rotation);
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

            Universe.QuestBoard.NotifyDeath(this, hd.Owner);

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
        var fleet = Universe.FleetManager.GetFleetOf(this);

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

    private void OnUndocked()
    {
        ResetControls();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        var fleet = Universe.FleetManager.GetFleetOf(this);
        if (fleet)
        {
            if (fleet.Leader == this)
            {
                foreach (var follower in fleet.Followers)
                {
                    var formationPos = fleet.GetFormationPos(follower);
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(transform.position, formationPos);
                    UnityEditor.Handles.Label(formationPos, "Formation Pos #" +fleet.Followers.IndexOf(follower));
                }
            }
            else
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, fleet.Leader.transform.position);
            }
        }
    }
#endif
}

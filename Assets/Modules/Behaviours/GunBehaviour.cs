#pragma warning disable 0649

using System;
using UnityEngine;

public class GunBehaviour : ModuleBehaviour, IWeapon
{
    private const string DESCRIPTION_FORMAT =
@"<color=#ffffffaa>Damage: </color> {0}-{1}
<color=#ffffffaa>Range:</color> {2:0.0}m
<color=#ffffffaa>Projectile speed:</color> {3:0.0}m/s";

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Create/SpaceTrader/Modules/Gun Behavior")]
    public static void CreateGunBehavior()
    {
        ScriptableObjectUtility.CreateAsset<GunBehaviour>();
    }
#endif

    [SerializeField]
	private Bullet bulletType;

    [SerializeField]
	private Transform muzzleFlashType;

    [SerializeField]
	private int minDamage;

    [SerializeField]
    private int maxDamage;
    
    [SerializeField]
    private float fireRate = 0.2f;
    
    public override string Description
    {
        get
        {
            float velocity = bulletType.velocity;
            float range = velocity * bulletType.lifetime;
            return string.Format(DESCRIPTION_FORMAT, minDamage, maxDamage, range, velocity);
        }
    }

    public int CalculateDps(Ship owner)
    {
        return (int) Mathf.Ceil((minDamage + maxDamage / 2) / fireRate);
    }
    
    public override void Equip(HardpointModule slot)
    {
        Cooldown = 0;
    }

    private int CalculateDamage(Ship activator, int slot)
    {
        float randomBase = UnityEngine.Random.Range(minDamage, maxDamage);

        float crewBonus;

        if (activator.CrewAssignments.Captain != null)
        {
            crewBonus = activator.CrewAssignments.Captain.WeaponsSkill * 0.1f;
        }
        else
        {
            crewBonus = 0;
        }

        float crewMultiplier = 1.0f + Mathf.Max(0.0f, crewBonus);

        return Mathf.FloorToInt(randomBase * crewMultiplier);
    }

    public override void Activate(Ship activator, int slot)
	{
        if (Cooldown > 0)
        {
            return;
        }

        var hardpoint = activator.GetHardpointAt(slot);
        var module = activator.ModuleLoadout.GetSlot(slot);

        var aimingAt = module.Aim;
        if (hardpoint.CanAimAt(aimingAt))
        {
            var firedTransform = hardpoint ? hardpoint.transform : activator.transform;

            var aimRot = Quaternion.LookRotation((module.Aim - firedTransform.position).normalized);

            var bulletInstance = (Bullet)Instantiate(bulletType, firedTransform.position, aimRot);

            bulletInstance.owner = activator.gameObject;
            bulletInstance.damage = CalculateDamage(activator, slot);

            if (activator.GetComponent<Rigidbody>())
            {
                bulletInstance.baseVelocity = activator.GetComponent<Rigidbody>().velocity;
            }

            if (muzzleFlashType)
            {
                var flash = (Transform)Instantiate(muzzleFlashType, firedTransform.position, firedTransform.rotation);
                flash.SetParent(firedTransform, true);
            }

            Cooldown = fireRate;
        }        
	}
    
    public override Vector3? PredictTarget(Ship activator, int slot, Targetable target)
    {
        Vector3 speedDiff;
        var targetBody = target.GetComponent<Rigidbody>();
        var activatorBody = activator.GetComponent<Rigidbody>();

        if (bulletType.applyBaseVelocity)
        {
            if (targetBody && activatorBody)
            {
                speedDiff = targetBody.velocity - activatorBody.velocity;
            }
            else if (activatorBody)
            {
                speedDiff = -activatorBody.velocity;
            }
            else
            {
                speedDiff = Vector3.zero;
            }
        }
        else
        {
            if (targetBody)
            {
                speedDiff = targetBody.velocity;
            }
            else
            {
                speedDiff = Vector3.zero;
            }
        }

        float distToTarget = (target.transform.position - activator.transform.position).magnitude;
        float timeToHit = distToTarget / bulletType.velocity;

        speedDiff *= timeToHit;

        var targetPos = target.transform.position;
        targetPos += speedDiff;

        return targetPos;
    } 
}

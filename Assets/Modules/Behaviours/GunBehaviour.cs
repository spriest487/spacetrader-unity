#pragma warning disable 0649

using UnityEngine;

[CreateAssetMenu(menuName = "SpaceTrader/Modules/Gun Behaviour")]
public class GunBehaviour : ModuleBehaviour
{
    private const string DESCRIPTION_FORMAT =
@"<color=#ffffffaa>Damage: </color> {0}-{1}
<color=#ffffffaa>Range:</color> {2:0.0}m
<color=#ffffffaa>Projectile speed:</color> {3:0.0}m/s";
    
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

    public override float CalculateDps(Ship owner)
    {
        var meanDamage = (minDamage + maxDamage) / 2f;

        meanDamage = owner.ApplyDamageModifier(Mathf.FloorToInt(meanDamage));

        return meanDamage / fireRate;
    }
    
    public override void Equip(HardpointModule slot)
    {
        Cooldown = 0;
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

            bulletInstance.owner = activator;
            var randomDamage = UnityEngine.Random.Range(minDamage, maxDamage);
            bulletInstance.damage = activator.ApplyDamageModifier(randomDamage);

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

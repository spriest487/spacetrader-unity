using UnityEngine;

public class GunBehaviour : ModuleBehaviour
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Create/SpaceTrader/Modules/Gun Behavior")]
    public static void CreateModuleDefinition()
    {
        ScriptableObjectUtility.CreateAsset<GunBehaviour>();
    }
#endif

    [SerializeField]
	private Bullet bulletType;

    [SerializeField]
	private Transform muzzleFlashType;

    [SerializeField]
    [HideInInspector]
	private int damagePerShot;

	public static GunBehaviour Create(int damagePerShot,
		Bullet bullet,
		Transform muzzleFlash)
	{
        GunBehaviour result = CreateInstance<GunBehaviour>();
        result.damagePerShot = damagePerShot;
        result.bulletType = bullet;
        result.muzzleFlashType = muzzleFlash;
        return result;
	}

	public override void Activate(Ship activator, WeaponHardpoint hardpoint, ModuleStatus module)
	{
        var aimingAt = module.Aim;
        if (hardpoint.CanAimAt(aimingAt))
        {
            var firedTransform = hardpoint ? hardpoint.transform : activator.transform;

            var aimRot = Quaternion.LookRotation((module.Aim - firedTransform.position).normalized);

            var bulletInstance = (Bullet)Instantiate(bulletType, firedTransform.position, aimRot);

            bulletInstance.owner = activator.gameObject;
            bulletInstance.damage = damagePerShot;

            if (activator.GetComponent<Rigidbody>())
            {
                bulletInstance.baseVelocity = activator.GetComponent<Rigidbody>().velocity;
            }

            if (muzzleFlashType)
            {
                var flash = (Transform)Instantiate(muzzleFlashType, firedTransform.position, firedTransform.rotation);
                flash.SetParent(firedTransform, true);
            }
        }        
	}
    
    public override Vector3? PredictTarget(Ship activator, WeaponHardpoint hardpoint, Targetable target)
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

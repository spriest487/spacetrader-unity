using UnityEngine;

public class GunBehaviour : ModuleBehaviour
{
    [SerializeField]
	private Transform bulletType;

    [SerializeField]
	private Transform muzzleFlashType;

    [SerializeField]
    [HideInInspector]
	private int damagePerShot;

	public static GunBehaviour Create(int damagePerShot,
		Transform bullet,
		Transform muzzleFlash)
	{
        GunBehaviour result = CreateInstance<GunBehaviour>();
        result.damagePerShot = damagePerShot;
        result.bulletType = bullet;
        result.muzzleFlashType = muzzleFlash;
        return result;
	}

	public override void Activate(Ship activator, WeaponHardpoint hardpoint)
	{
        var firedTransform = hardpoint ? hardpoint.transform : activator.transform;

        var aimRot = Quaternion.LookRotation((activator.aim - firedTransform.position).normalized);

		var bulletInstance = (Transform) Instantiate(bulletType, firedTransform.position, aimRot);
		var bulletBehaviour = bulletInstance.GetComponent<Bullet>();
		if (bulletBehaviour)
		{
			bulletBehaviour.owner = activator.gameObject;
			bulletBehaviour.damage = damagePerShot;

			if (activator.GetComponent<Rigidbody>())
			{
				bulletBehaviour.baseVelocity = activator.GetComponent<Rigidbody>().velocity;
			}
		}

		if (muzzleFlashType)
		{
			var flash = (Transform) Instantiate(muzzleFlashType, firedTransform.position, firedTransform.rotation);
            flash.SetParent(firedTransform, true);
		}
	}
}

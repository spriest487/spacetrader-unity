using UnityEngine;

public class GunBehaviour : ModuleBehaviour
{
    [SerializeField]
	private Transform bulletType;

    [SerializeField]
	private Transform muzzleFlashType;

	private int damagePerShot;

	public static GunBehaviour Create(int damagePerShot,
		Transform bullet,
		Transform muzzleFlash)
	{
        GunBehaviour result = ScriptableObject.CreateInstance<GunBehaviour>();
        result.damagePerShot = damagePerShot;
        result.bulletType = bullet;
        result.muzzleFlashType = muzzleFlash;
        return result;
	}

	public override void Activate(Ship activator, WeaponHardpoint hardpoint)
	{
		var aimRot = Quaternion.LookRotation((activator.aim - hardpoint.transform.position).normalized);

		var bulletInstance = (Transform) GameObject.Instantiate(bulletType, hardpoint.transform.position, aimRot);
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
			var flash = (Transform) GameObject.Instantiate(muzzleFlashType, hardpoint.transform.position, hardpoint.transform.rotation);
			flash.parent = hardpoint.transform;
		}
	}
}

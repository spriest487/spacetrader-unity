using UnityEngine;

public class GunBehaviour : ModuleBehaviour
{
	private Transform bulletType;
	private Transform muzzleFlashType;

	private int damagePerShot;

	public GunBehaviour(int damagePerShot,
		Transform bullet,
		Transform muzzleFlash)
	{
		this.damagePerShot = damagePerShot;
		this.bulletType = bullet;
		this.muzzleFlashType = muzzleFlash;
	}

	public void Activate(Ship activator, WeaponHardpoint hardpoint)
	{
		var aimRot = Quaternion.LookRotation((activator.aim - hardpoint.transform.position).normalized);

		var bulletInstance = (Transform) GameObject.Instantiate(bulletType, hardpoint.transform.position, aimRot);
		var bulletBehaviour = bulletInstance.GetComponent<Bullet>();
		if (bulletBehaviour)
		{
			bulletBehaviour.owner = activator.gameObject;
			bulletBehaviour.damage = damagePerShot;

			if (activator.rigidbody)
			{
				bulletBehaviour.baseVelocity = activator.rigidbody.velocity;
			}
		}

		if (muzzleFlashType)
		{
			var flash = (Transform) GameObject.Instantiate(muzzleFlashType, hardpoint.transform.position, hardpoint.transform.rotation);
			flash.parent = hardpoint.transform;
		}
	}
}

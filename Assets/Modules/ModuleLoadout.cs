using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Ship))]
public class ModuleLoadout : MonoBehaviour
{
	[System.Serializable]
	public struct ModuleStatus
	{
		public string moduleName;
		public float cooldown;
	}

	public ModuleStatus[] modules;

	private ModuleConfiguration moduleConfig;
	private Ship ship;

	private WeaponHardpoint[] hardpoints;
	
	public void Activate(int index)
	{
		if (index < 0 || index >= modules.Length)
		{
			throw new UnityException("Bad index in module loadout: " +index);
		}

		if (modules[index].cooldown <= Mathf.Epsilon) //cd check
		{
			foreach (var hardpoint in hardpoints)
			{
				var aimRot = Quaternion.LookRotation((ship.aim - hardpoint.transform.position).normalized);

				var bullet = (Transform) Instantiate(moduleConfig.bullet, hardpoint.transform.position, aimRot);
				var bulletBehaviour = bullet.GetComponent<Bullet>();
				if (bulletBehaviour)
				{
					bulletBehaviour.owner = gameObject;
				}

				if (moduleConfig.bulletMuzzleFlash)
				{
					var flash = (Transform) Instantiate(moduleConfig.bulletMuzzleFlash, hardpoint.transform.position, hardpoint.transform.rotation);
					flash.parent = hardpoint.transform;
				}

				modules[index].cooldown = 0.2f;
			}
		}
	}

	void Start()
	{
		var config = GameObject.FindGameObjectWithTag("WorldConfig");
		moduleConfig = config.GetComponent<ModuleConfiguration>();

		ship = GetComponent<Ship>();
		hardpoints = ship.GetComponentsInChildren<WeaponHardpoint>();
	}

	void Update()
	{
		for (int module = 0; module < modules.Length; ++module)
		{
			modules[module].cooldown -= Time.deltaTime;
		}
	}
}

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Bullet : MonoBehaviour {
	public GameObject owner;
	
	public Transform hitEffect;
	
	public float velocity;
	public float lifetime;
	
	private Vector3 lastPos;
	
	void FixedUpdate()
	{
		lastPos = transform.position;

		var frameSpeed = velocity * Time.deltaTime;

		bool dead = false;

		int raycastMask = ~LayerMask.GetMask("Bullets and Effects");
		RaycastHit hit;
		if (Physics.Raycast(lastPos, transform.forward, out hit, frameSpeed, raycastMask))
		{
			var hitPos = hit.point;
			var hitObj = hit.collider.gameObject;

			dead = Hit(hitObj, hitPos);
		}

		if (!dead)
		{
			transform.position += transform.forward * frameSpeed;

			lifetime -= Time.deltaTime;
			if (lifetime < 0)
			{
				Destroy(gameObject);
			}
		}
	}

	void Start()
	{
		if (!collider.isTrigger)
		{
			//Debug.Log("Bullet objects should be triggers, not solid colliders");
		}
	}

	void OnTriggerEnter(Collider triggered)
	{
		Hit(triggered.gameObject, transform.position);
	}

	private bool Hit(GameObject hitObject, Vector3 hitPos)
	{
		if (hitObject != owner && hitObject != gameObject) //bullets are never triggered by the person shooting them
		{
			//Debug.Log(string.Format("bullet with owner {0} hit non-owner object {1}", owner, hitObject));

			if (hitEffect)
			{
				Instantiate(hitEffect, hitPos, Quaternion.LookRotation((lastPos - hitPos).normalized));
			}

			Destroy(gameObject);
			return true;
		}

		return false;
	}
}

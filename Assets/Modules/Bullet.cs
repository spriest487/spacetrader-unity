using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Bullet : MonoBehaviour {
	public GameObject owner;
	
	public Transform hitEffect;
	
	public float velocity;
	public float lifetime;

    public Vector3 baseVelocity;
    public bool applyBaseVelocity;
	
	private Vector3 lastPos;
	
	void FixedUpdate()
	{
        lastPos = transform.position;

		var frameSpeed = transform.forward * (velocity * Time.deltaTime);

        if (applyBaseVelocity)
        {
            frameSpeed += baseVelocity * Time.deltaTime;
        }

        var frameDistance = frameSpeed.magnitude;
        var frameDirection = frameSpeed / frameDistance;

		int raycastMask = ~LayerMask.GetMask("Bullets and Effects", "Ignore Raycast");
        
        Vector3 nextPos;
        RaycastHit hit;
        bool dead;

		if (Physics.Raycast(lastPos, frameDirection, out hit, frameDistance, raycastMask))
		{
			nextPos = hit.point;
			var hitObj = hit.collider.gameObject;

            dead = Hit(hitObj, nextPos);
		}
        else
        {
            nextPos = lastPos + frameSpeed;
            dead = false;
        }

		if (!dead)
		{
            transform.position = nextPos;

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

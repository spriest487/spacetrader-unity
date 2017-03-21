using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Bullet : MonoBehaviour
{
	public Ship owner;
	
	public Transform hitEffect;

	public int damage;
	
	public float velocity;
	public float lifetime;

    public Vector3 baseVelocity;
    public bool applyBaseVelocity;
	
	private Vector3 lastPos;

    private int rayMask;

    void Awake()
    {
        rayMask = ~LayerMask.GetMask("Bullets and Effects", "Ignore Raycast");
    }
    
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
                
        Vector3 nextPos;
        RaycastHit hit;
        bool dead;
        
		if (Physics.Raycast(lastPos, 
            frameDirection, 
            out hit, 
            frameDistance,
            rayMask, 
            QueryTriggerInteraction.Ignore))
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

	void OnTriggerEnter(Collider triggered)
	{
        var triggeredMask = 1 << triggered.gameObject.layer;
        if ((rayMask & triggeredMask) == triggeredMask)
        {
            Hit(triggered.gameObject, transform.position);
        }
	}

	private bool Hit(GameObject hitObject, Vector3 hitPos)
	{
        //bullets are never triggered by the person shooting them
        var hitShip = hitObject.GetComponentInParent<Ship>();

		if (hitShip && hitShip != owner) 
		{
            var hitDamage = new HitDamage(hitPos, damage, owner);
			hitShip.gameObject.SendMessage("OnTakeDamage", hitDamage, SendMessageOptions.DontRequireReceiver);

            //if a handler changed it
            hitPos = hitDamage.Location;

            if (hitEffect)
            {
                var hitRotation = Quaternion.LookRotation(transform.forward);
                Instantiate(hitEffect, hitPos, hitRotation);
            }

			Destroy(gameObject);
			return true;
		}

		return false;
	}
}

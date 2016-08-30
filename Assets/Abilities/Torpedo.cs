using UnityEngine;

[RequireComponent(typeof(Ship))]
public class Torpedo : MonoBehaviour
{
    [SerializeField]
    private Ship owner;

    [SerializeField]
    private float explodeProximity = 5;

    [SerializeField]
    private int explodeDamage = 10;

    [SerializeField]
    private float lifetime = 10;

    public Ship Owner
    {
        get { return owner; }
        set { owner = value; }
    }
    
    private Ship torpedoShip;

    void Start()
    {
        torpedoShip = GetComponent<Ship>();
    }

    public void UpdateCollisions()
    {
        if (!owner)
        {
            return;
        }

        var myCollider = GetComponent<Collider>();
        var ownerCollider = owner.GetComponent<Collider>();

        if (myCollider && ownerCollider)
        {
            Physics.IgnoreCollision(myCollider, ownerCollider);
        }
    }

    void Explode()
    {
        torpedoShip.Explode();
    }

    void Update()
    {
        lifetime -= Time.deltaTime;

        if (lifetime < 0)
        {
            Explode();
        }
        else if (torpedoShip.Target)
        {
            /* if target dies, the ship will just keep moving towards its
            last position */
            var toTarget = torpedoShip.Target.transform.position - transform.position;
            
            var dist2 = toTarget.sqrMagnitude;
            var proximity2 = explodeProximity * explodeProximity;

            if (dist2 < proximity2)
            {
                //TODO: damage in a sphere instead
                var damage = new HitDamage(transform.position, explodeDamage, owner);

                torpedoShip.Target.SendMessage("OnTakeDamage", damage, SendMessageOptions.DontRequireReceiver);

                Explode();
            }
            else
            {
                torpedoShip.ResetControls(thrust: 1);
                torpedoShip.RotateToDirection(toTarget.normalized);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Explode();
    }
}
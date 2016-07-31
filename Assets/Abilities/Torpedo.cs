using UnityEngine;

[RequireComponent(typeof(AICaptain))]
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

    private AICaptain captain;

    public Ship Owner
    {
        get { return owner; }
        set { owner = value; }
    }

    void Start()
    {
        captain = GetComponent<AICaptain>();
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
        captain.Ship.Explode();
    }

    void Update()
    {
        lifetime -= Time.deltaTime;

        if (lifetime < 0)
        {
            Explode();
        }
        else
        {
            /* if target dies, the ai will just keep moving towards its
               last position*/
            if (captain.Ship.Target)
            {
                captain.Destination = captain.Ship.Target.transform.position;
            }
            captain.Throttle = 1f;
            captain.MinimumThrust = 1f;

            var dist2 = (transform.position - captain.Destination).sqrMagnitude;
            var proximity2 = explodeProximity * explodeProximity;

            if (dist2 < proximity2)
            {
                //TODO: damage in a sphere instead
                if (captain.Ship.Target)
                {
                    var damage = new HitDamage(transform.position, explodeDamage, owner ? owner.gameObject : null);

                    captain.Ship.Target.gameObject.SendMessage("OnTakeDamage", damage, SendMessageOptions.DontRequireReceiver);
                }

                Explode();
            }
        }        
    }

    void OnCollisionEnter(Collision collision)
    {
        Explode();
    }
}
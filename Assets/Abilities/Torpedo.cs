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
    }

    public Ship Ship
    {
        get { return torpedoShip; }
    }
    
    private Ship torpedoShip;

    public static Torpedo CreateFromShip(Ship ship, Ship owner)
    {
        var torpedo = ship.gameObject.AddComponent<Torpedo>();
        torpedo.torpedoShip = ship;
        torpedo.owner = owner;
        ship.Target = owner.Target;

        Physics.IgnoreCollision(ship.GetComponent<Collider>(), owner.GetComponent<Collider>());

        return torpedo;
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
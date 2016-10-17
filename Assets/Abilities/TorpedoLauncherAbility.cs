#pragma warning disable 0649

using UnityEngine;

[CreateAssetMenu(menuName = "SpaceTrader/Abilities/Torpedo Launcher")]
public class TorpedoLauncherAbility : Ability
{
    [SerializeField]
    private ShipType torpedoType;

    [SerializeField]
    private float cooldownLength = 5;

    public override bool Cancellable
    {
        get { return false; }
    }

    public override void Use(Ship ship)
    {
        if (Cooldown > 0 || !ship.Target || ship.Target.TargetSpace != TargetSpace.Local)
        {
            return;
        }

        var torpedoShip = torpedoType.CreateShip(ship.transform.position, ship.transform.rotation);
        var torpedo = Torpedo.CreateFromShip(torpedoShip, ship);

        var torpedoRb = torpedo.Ship.RigidBody;
        var ownerRb = ship.RigidBody;
        if (torpedoRb && ownerRb)
        {
            torpedoRb.velocity = ownerRb.velocity;
            torpedoRb.angularVelocity = ownerRb.angularVelocity;
        }

        Cooldown = cooldownLength;
    }

    public override string Describe()
    {
        return "Fires a seeking torpedo at the current target";
    }
}

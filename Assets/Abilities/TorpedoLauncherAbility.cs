#pragma warning disable 0649

using System;
using UnityEngine;

public class TorpedoLauncherAbility : Ability
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Create/SpaceTrader/Abilities/Torpedo Launcher")]
    public static void CreateAbilityDefinition()
    {
        ScriptableObjectUtility.CreateAsset<TorpedoLauncherAbility>();
    }
#endif

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
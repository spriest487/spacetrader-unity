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
    private Torpedo torpedoPrefab;

    [SerializeField]
    private float cooldownLength = 5;

    public override bool Cancellable
    {
        get { return false; }
    }

    public override void Use(Ship ship)
    {
        if (Cooldown > 0 || !ship.Target)
        {
            return;
        }

        var torpedo = (Torpedo) Instantiate(torpedoPrefab, ship.transform.position, ship.transform.rotation);
        torpedo.GetComponent<Ship>().Target = ship.Target;
        torpedo.Owner = ship;

        var torpedoRb = torpedo.GetComponent<Rigidbody>();
        var ownerRb = ship.GetComponent<Rigidbody>();
        if (torpedoRb && ownerRb)
        {
            torpedoRb.velocity = ownerRb.velocity;
            torpedoRb.angularVelocity = ownerRb.angularVelocity;
        }

        torpedo.UpdateCollisions();

        Cooldown = cooldownLength;
    }

    public override string Describe()
    {
        return "Fires a seeking torpedo at the current target";
    }
}
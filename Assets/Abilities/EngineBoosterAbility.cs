#pragma warning disable 0649

using UnityEngine;

[CreateAssetMenu(menuName = "SpaceTrader/Abilities/Engine Booster")]
public class EngineBoosterAbility : Ability
{
    [SerializeField, HideInInspector]
    private StatusEffect activeEffect;

    [SerializeField]
    private float boostAmount = 10;

    [SerializeField]
    private float boostDuration = 5;

    [SerializeField]
    private float boostCooldown = 10;

    public override void Use(Ship ship)
    {
        if (!(Cooldown > 0 || activeEffect != null))
        {
            var boost = new ShipStats
            {
                MaxSpeed = boostAmount,
                Thrust = boostAmount
            };

            Cooldown = boostCooldown;

            activeEffect = new StatusEffect("Speed Boost", Time.time + boostDuration, boost, null);
            ship.AddStatusEffect(activeEffect);
        }
    }

    public override void Cancel(Ship ship)
    {
        if (activeEffect != null)
        {
            if (!ship.RemoveStatusEffect(activeEffect))
            {
                Debug.LogWarning("tried to cancel a booster effect but it wasn't affecting the target ship");
            }
        }
        else
        {
            Debug.LogWarning("tried to cancel a booster effect that wasn't active");
        }

        activeEffect = null;
    }

    public override string Describe()
    {
        return string.Format("Boost speed and max speed by {0:n1} for {1:n2} seconds",
            boostAmount,
            boostDuration);
    }
}

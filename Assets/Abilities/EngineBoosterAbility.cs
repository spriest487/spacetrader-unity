using System;
using UnityEngine;

public class EngineBoosterAbility : Ability
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Create/SpaceTrader/Abilities/Engine Booster")]
    public static void CreateAbilityDefinition()
    {
        ScriptableObjectUtility.CreateAsset<EngineBoosterAbility>();
    }
#endif

    [SerializeField, HideInInspector]
    private StatusEffect activeEffect;

    [SerializeField]
    private float boostAmount = 10;

    [SerializeField]
    private float boostDuration = 3;

    public override void Use(Ship ship)
    {
        if (!(Cooldown > 0 || activeEffect))
        {
            activeEffect = CreateInstance<StatusEffect>();
            activeEffect.FlatStats.maxSpeed = boostAmount;
            activeEffect.FlatStats.thrust = boostAmount;
            activeEffect.Expires = true;
            activeEffect.Lifetime = boostDuration;

            Cooldown = activeEffect.Lifetime;

            ship.AddStatusEffect(activeEffect);
        }
    }

    public override void Cancel(Ship ship)
    {
        if (activeEffect)
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
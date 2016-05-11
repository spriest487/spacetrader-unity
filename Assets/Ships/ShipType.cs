#pragma warning disable 0649

using UnityEngine;
using System.Collections.Generic;

public class ShipType : ScriptableObject
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Create/SpaceTrader/Ship Type")]
    public static void Create()
    {
        ScriptableObjectUtility.CreateAsset<ShipType>();
    }
#endif

    [SerializeField]
    private Transform prefab;

    [SerializeField]
    private ShipStats stats;
    
    [SerializeField]
    private int armor;

    [SerializeField]
    private int[] shieldSectors;

    [SerializeField]
    private int cargoSize;

    [SerializeField]
    private bool moorable;

    [SerializeField]
    private List<Ability> abilities;

    [SerializeField]
    private ScalableParticle explosionEffect;

    public ShipStats Stats
    {
        get
        {
            return stats;
        }
    }

    public Ship CreateShip(Vector3 position, Quaternion rotation)
    {
        var obj = (Transform) Instantiate(prefab, position, rotation);
        obj.name = this.name;

        var ship = obj.gameObject.AddComponent<Ship>();
        ship.BaseStats = stats;
        ship.ExplosionEffect = explosionEffect;

        var newAbilities = new List<Ability>();
        foreach (var ability in abilities)
        {
            if (ability != null)
            {
                var abilityInstance = Instantiate(ability);
                abilityInstance.name = ability.name;
                abilityInstance.Cooldown = 0;
                newAbilities.Add(abilityInstance);
            }
        }
        ship.Abilities = newAbilities;

        if (moorable && !obj.gameObject.GetComponent<Moorable>())
        {
            obj.gameObject.AddComponent<Moorable>();
        }   
                
        if (cargoSize > 0)
        {
            ship.Cargo = CreateInstance<CargoHold>();
            ship.Cargo.Size = cargoSize;
        }
        else
        {
            ship.Cargo = null;
        }

        var hp = obj.gameObject.AddComponent<Hitpoints>();
        hp.Reset(armor, shieldSectors);

        return ship;
    }
}

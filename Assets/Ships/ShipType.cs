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
    private Camera cockpitPrefab;

    [SerializeField]
    private ShipStats stats;
    
    [SerializeField]
    private int cargoSize;

    [SerializeField]
    private int moduleSlots;

    [SerializeField]
    private bool moorable;

    [SerializeField]
    private List<Ability> abilities;
    
    [SerializeField]
    private ScalableParticle explosionEffect;

    [SerializeField]
    private int xpReward;

    public ScalableParticle ExplosionEffect { get { return explosionEffect; } }
    public ShipStats Stats { get { return stats; } }
    public IEnumerable<Ability> Abilities { get { return abilities; } }
    public bool Moorable { get { return moorable; } }
    public int CargoSize { get { return cargoSize; } }
    public int ModuleSlots { get { return moduleSlots; } }
    public bool HasCockpit { get { return !!cockpitPrefab; } }
    public int XPReward { get { return xpReward; } }

    public Ship CreateShip(Vector3 position, Quaternion rotation)
    {
        var obj = (Transform) Instantiate(prefab, position, rotation);
        obj.name = this.name;

        var ship = Ship.Create(obj.gameObject, this);
        
        return ship;
    }

    public Camera CreateCockpit()
    {
        return (Camera) Instantiate(cockpitPrefab, Vector3.zero, Quaternion.identity);
    }
}

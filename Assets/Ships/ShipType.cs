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

    [Header("Prefabs")]

    [SerializeField]
    private Transform prefab;

    [SerializeField]
    private Camera cockpitPrefab;

    [SerializeField]
    private ScalableParticle explosionEffect;

    [Header("Ship Details")]

    [SerializeField]
    private bool targetable = true;

    [SerializeField]
    private ShipStats stats = new ShipStats();
    
    [SerializeField]
    private int cargoSize = 1;

    [SerializeField]
    private int moduleSlots;

    [SerializeField]
    private bool moorable = true;

    [SerializeField]
    private List<Ability> abilities;
    
    [SerializeField]
    private int xpReward;

    public ScalableParticle ExplosionEffect { get { return explosionEffect; } }
    public ShipStats Stats { get { return stats; } }
    public IEnumerable<Ability> Abilities { get { return abilities; } }
    public bool Targetable { get { return targetable; } }
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

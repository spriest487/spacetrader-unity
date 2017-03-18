#pragma warning disable 0649

using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "SpaceTrader/Ship Type")]
public class ShipType : ScriptableObject
{
    [Header("Prefabs")]

    [SerializeField]
    private Transform prefab;

    [SerializeField]
    private CockpitView cockpitPrefab;

    [SerializeField]
    private ScalableParticle explosionEffect;

    [SerializeField]
    private Transform miniThrusterEffect;

    [SerializeField]
    private Transform thrusterEffect;

    [Header("UI")]

    [SerializeField]
    private Sprite thumbnail;

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
    private bool dockable = true;

    [SerializeField]
    private List<Ability> abilities;

    [SerializeField]
    private int xpReward;

    public Transform Prefab { get { return prefab; } }

    public ScalableParticle ExplosionEffect { get { return explosionEffect; } }
    public Transform MiniThrusterEffect { get { return miniThrusterEffect ?? thrusterEffect; } }
    public Transform ThrusterEffect { get { return thrusterEffect;  } }

    public ShipStats Stats { get { return stats; } }
    public IEnumerable<Ability> Abilities { get { return abilities; } }
    public bool Targetable { get { return targetable; } }
    public bool Dockable { get { return dockable; } }
    public int CargoSize { get { return cargoSize; } }
    public int ModuleSlots { get { return moduleSlots; } }
    public bool HasCockpit { get { return !!cockpitPrefab; } }
    public int XPReward { get { return xpReward; } }
    public Sprite Thumbnail { get { return thumbnail; } }

    public Ship CreateShip(Vector3 position, Quaternion rotation)
    {
        var obj = Instantiate(prefab, position, rotation);
        obj.name = name;
        var ship = Ship.Create(obj.gameObject, this);

        return ship;
    }

    public CockpitView CreateCockpit(FollowCamera playerCam)
    {
        return CockpitView.Create(cockpitPrefab, playerCam);
    }
}

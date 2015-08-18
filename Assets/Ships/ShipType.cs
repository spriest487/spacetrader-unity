using UnityEngine;
using System.Collections;

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
    private ScalableParticle explosionEffect;

    public Ship CreateShip(Vector3 position, Quaternion rotation)
    {
        var obj = (Transform) Instantiate(prefab, position, rotation);

        var ship = obj.gameObject.AddComponent<Ship>();
        ship.Stats = stats;
        ship.ExplosionEffect = explosionEffect;

        if (moorable)
        {
            obj.gameObject.AddComponent<Moorable>();
        }   

        var cargo = obj.gameObject.AddComponent<CargoHold>();

        var hp = obj.gameObject.AddComponent<Hitpoints>();
        hp.Reset(armor, shieldSectors);

        return ship;
    }
}

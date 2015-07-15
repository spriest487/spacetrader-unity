using UnityEngine;
using System.Collections;

public class ShipType : ScriptableObject
{
    [SerializeField]
    private Transform prefab;

    [SerializeField]
    private ShipStats stats;

    [SerializeField]
    private string typeName;

    [SerializeField]
    private int hardpoints;

    [SerializeField]
    private int modules;

    [SerializeField]
    private int armor;

    [SerializeField]
    private int[] shieldSectors;

    public Ship CreateShip(Vector3 position, Quaternion rotation)
    {
        var obj = (Transform) Instantiate(prefab, position, rotation);

        var ship = obj.gameObject.AddComponent<Ship>();
        ship.Stats = stats;

        var loadout = obj.gameObject.AddComponent<ModuleLoadout>();

        var moorable = obj.gameObject.AddComponent<Moorable>();

        var hp = obj.gameObject.AddComponent<Hitpoints>();
        hp.Reset(armor, shieldSectors);

        return ship;
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Create/SpaceTrader/Ship Type")]
    public static void Create()
    {
        ScriptableObjectUtility.CreateAsset<ShipType>();
    }
#endif
}

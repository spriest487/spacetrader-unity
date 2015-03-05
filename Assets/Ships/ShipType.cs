using UnityEngine;
using System.Collections;

public class ShipType : ScriptableObject
{
    [SerializeField]
    private Transform prefab;

    [SerializeField]
    private string typeName;

    [SerializeField]
    private int hardpoints;

    [SerializeField]
    private int modules;

    public Ship CreateShip()
    {
        var obj = (Transform) Instantiate(prefab);

        var ship = obj.gameObject.AddComponent<Ship>();
        var loadout = obj.gameObject.AddComponent<ModuleLoadout>();

        var moorable = obj.gameObject.AddComponent<Moorable>();

        return ship;
    }
}

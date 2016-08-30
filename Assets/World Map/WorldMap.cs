using UnityEngine;
using System;
using System.Collections.Generic;

public class WorldMap : ScriptableObject
{
    [Serializable]
    public class Area
    {
        public string Name;
        public Vector3 Position;
    }

    private List<Area> areas;

    public IEnumerable<Area> Areas { get { return areas; } }

    public static WorldMap Create(IEnumerable<Area> areas)
    {
        var map = CreateInstance<WorldMap>();
        map.areas = new List<Area>(areas);

        return map;
    }

    public static WorldMap Create(WorldMap prefab)
    {
        return Instantiate(prefab);
    }
}


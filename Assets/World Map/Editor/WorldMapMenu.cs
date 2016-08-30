using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;

public static class WorldMapMenu
{
    [MenuItem("SpaceTrader/Save scene as world map")]
    public static void GenerateWorldMap()
    {
        var areas = GameObject.FindObjectsOfType<WorldMapArea>()
            .Select(a => new WorldMap.Area
            {
                Name = a.name,
                Position = a.transform.position
            });
    }
}

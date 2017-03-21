using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class GameObjectUtility
{
    public static void WalkChildren(this GameObject obj, Action<GameObject> action)
    {
        action(obj);
        int count = obj.transform.childCount;
        for (int child = 0; child < count; ++child)
        {
            obj.transform.GetChild(child).gameObject.WalkChildren(action);
        }
    }

    public static int DistanceComparison<T>(this Transform xform, T a, T b)
        where T : Component
    {
        var toA = a.transform.position - xform.position;
        var toB = b.transform.position - xform.position;

        return toA.sqrMagnitude.CompareTo(toB.sqrMagnitude);
    }

    public static T Closest<T>(this Transform xform, IEnumerable<T> from)
        where T : Component
    {
        if (from == null || !from.Any())
        {
            return null;
        }

        var sorted = new List<T>(from);
        sorted.Sort(xform.DistanceComparison);

        return sorted.First();
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawPrefabWire(this GameObject prefab, Transform baseTransform,
        Vector3 position, Quaternion rotation, Vector3 localScale)
    {
        if (!prefab)
        {
            return;
        }

        var meshes = prefab.GetComponentsInChildren<MeshFilter>();
        foreach (var meshFilter in meshes)
        {
            if (!meshFilter.sharedMesh)
            {
                continue;
            }

            var meshXform = meshFilter.transform;
            var prefabRoot = meshXform.root;

            var pos = baseTransform.position 
                + position 
                + (meshXform.position - prefabRoot.position);
            var rot = baseTransform.rotation 
                * rotation 
                * meshXform.rotation 
                * Quaternion.Inverse(prefabRoot.rotation);
            var scale = baseTransform.lossyScale
                .Multiply(localScale)
                .Multiply(meshXform.lossyScale);

            Gizmos.DrawWireMesh(meshFilter.sharedMesh, pos, rot, scale);
        }
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawPrefabWire(this GameObject prefab, Transform baseTransform)
    {
        prefab.DrawPrefabWire(baseTransform, Vector3.zero, Quaternion.identity, Vector3.one);
    }
}

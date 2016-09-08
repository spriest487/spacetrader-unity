using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class GameObjectUtility
{
    public static void SetLayerRecursive(this GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            child.gameObject.SetLayerRecursive(layer);
        }
    }

    public static int DistanceComparison<T>(this Transform xform, T a, T b)
        where T : MonoBehaviour
    {
        var toA = a.transform.position - xform.position;
        var toB = b.transform.position - xform.position;

        return toA.sqrMagnitude.CompareTo(toB.sqrMagnitude);
    }
}

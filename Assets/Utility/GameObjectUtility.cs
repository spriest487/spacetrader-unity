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
}

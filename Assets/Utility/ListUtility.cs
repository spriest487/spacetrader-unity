using System.Collections.Generic;
using UnityEngine;

public static class ListUtility
{
    public static bool ElementsEquals<T>(this IList<T> list, IEnumerable<T> other)
    {
        var otherItems = new List<T>(other);

        if (other == null || list.Count != otherItems.Count)
        {
            return false;
        }

        var size = list.Count;
        for (int i = 0; i < size; ++i)
        {
            if (!Equals(list[i], otherItems[i]))
            {
                return false;
            }
        }

        return true;
    }

    public static int Resize<T>(this IList<T> list, int size) 
        where T : class
    {
        int diff = size - list.Count;

        if (diff > 0)
        {
            for (int i = 0; i < diff; ++i)
            {
                list.Add(default(T));
            }
        }
        else if (diff < 0)
        {
            for (int i = diff; i < 0; ++i)
            {
                list.RemoveAt(list.Count - 1);
            }
        }

        //number of elements added/removed
        return diff;
    }

    public static IEnumerable<T> AsEnumerable<T>(this T value)
    {
        yield return value;
    }

    public static IEnumerable<T> AsOptional<T>(this T value) 
        where T : class
    {
        if (value != null)
        {
            yield return value;
        }
        else
        {
            yield break;
        }
    }

    public static IEnumerable<T> AsOptionalObject<T>(this T obj) 
        where T : UnityEngine.Object
    {
        if (obj)
        {
            yield return obj;
        }
        else
        {
            yield break;
        }
    }

    public static T Random<T>(this IList<T> list)
    {
        Debug.Assert(list != null && list.Count > 0, "can't pick random element from empty or null list");

        var index = UnityEngine.Random.Range(0, list.Count);

        return list[index];
    }
}
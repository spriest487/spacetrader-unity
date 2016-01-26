using System.Collections.Generic;

internal static class ListUtility
{
    public static bool ElementsEquals<T>(this List<T> list, List<T> other)
    {
        if (other == null || list.Count != other.Count)
        {
            return false;
        }

        var size = list.Count;
        for (int i = 0; i < size; ++i)
        {
            if (!Equals(list[i], other[i]))
            {
                return false;
            }
        }

        return true;
    }

    public static void Resize<T>(this List<T> list, int size) 
        where T : class
    {
        int diff = size - list.Count;

        if (diff > 0)
        {
            for (int i = 0; i < diff; ++i)
            {
                list.Add(null);
            }
        }
        else if (diff < 0)
        {
            list.RemoveRange(list.Count - diff, diff);
        }
    }
}
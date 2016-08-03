using System.Collections.Generic;

internal static class ListUtility
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
}
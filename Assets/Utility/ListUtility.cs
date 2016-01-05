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
}
using System;
using System.Collections.Generic;

namespace Fort
{
    public static class ArrayHelper
    {
        public static int IndexOf<T>(this IEnumerable<T> collection, Predicate<T> predicate)
        {
            int index = 0;
            foreach (T obj in collection)
            {
                if (predicate(obj))
                    return index;
                ++index;
            }
            return -1;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace MapElites.Extensions
{
    public static class CollectionExtensions
    {
        public static T MaxBy<T>(this ICollection<T> collection, Func<T, float> selector)
        {
            if (collection.Count == 0)
            {
                throw new ArgumentException("Collection is empty");
            }
            
            T first = collection.First();
            
            float max = selector(first);
            T element = first;
            
            foreach (T t in collection)
            {
                if (selector(t) >= max)
                {
                    max = selector(t);
                    element = t;
                }
            }

            return element;
        }
    }
}
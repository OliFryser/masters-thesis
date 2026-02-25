using System;
using System.Collections.Generic;
using System.Linq;

namespace WFC.Extensions
{
    public static class CollectionExtensions
    {
        public static T GetRandomElement<T>(this ICollection<T> collection, Random random)
        {
            int size = collection.Count;
            int randomIndex = random.Next(size);
            return collection.ToList()[randomIndex];
        }

        public static T GetRandomElement<T>(this ICollection<T> collection) =>
            collection.GetRandomElement(new Random());

        public static T GetRandomWeightedElement<T>(this ICollection<T> collection, Random random)
        {
            throw new NotImplementedException();
        }

        public static T GetRandomWeightedElement<T>(this ICollection<T> collection)
        {
            throw new NotImplementedException();
        }

        public static T MinBy<T>(this IEnumerable<T> collection, Func<T, float> comparer)
        {
            T min = default;
            float minValue = float.MaxValue;
            foreach (T element in collection)
            {
                float value = comparer(element);
                if (value <= minValue)
                {
                    min = element;
                    minValue = value;
                }
            }

            return min ?? throw new InvalidOperationException("Collection is empty and could not find minimum.");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Domain;

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

        public static TileOption GetRandomWeightedElement(this ICollection<TileOption> collection, Random random)
        {
            throw new NotImplementedException();
        }

        public static TileOption GetRandomWeightedElement(this ICollection<TileOption> collection)
        {
            throw new NotImplementedException();
        }
    }
}
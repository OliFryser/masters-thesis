using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Extensions
{
    public static class CollectionExtensions
    {
        public static T GetRandomElement<T>(this ICollection<T> collection, Random random)
        {
            int size = collection.Count;
            int randomIndex = random.Next(size);
            return collection.ToList()[randomIndex];
        }
        
        public static T GetRandomElement<T>(this ICollection<T> collection)
            => GetRandomElement(collection, new Random());
    }
}
using System;
using System.Collections.Generic;
using WFC.Extensions;

namespace MapElites.Models
{
    public class Archive<TIndividual, TBehavior, TKey> where TKey : IEquatable<TKey>
    {
        private Dictionary<TKey, Entry<TIndividual, TBehavior>> _archive =
            new Dictionary<TKey, Entry<TIndividual, TBehavior>>();

        internal bool TryAdd(TKey key, Entry<TIndividual, TBehavior> entry)
        {
            if (_archive.TryGetValue(key, out Entry<TIndividual, TBehavior> existingEntry) 
                && entry.Fitness > existingEntry.Fitness)
            {
                _archive[key] = entry;
                return true;
            }
            
            return false;
        }

        internal TIndividual SampleRandom() => _archive.Values.GetRandomElement().Individual;
    }
}
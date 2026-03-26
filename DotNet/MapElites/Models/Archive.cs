using System;
using System.Collections.Generic;
using WFC.Extensions;

namespace MapElites.Models
{
    public class Archive<TIndividual, TBehavior, TKey> where TKey : IEquatable<TKey>
    {
        private readonly Dictionary<TKey, Entry<TIndividual, TBehavior>> _archive =
            new Dictionary<TKey, Entry<TIndividual, TBehavior>>();

        internal bool TryAdd(TKey key, Entry<TIndividual, TBehavior> entry)
        {
            if (_archive.TryGetValue(key, out Entry<TIndividual, TBehavior> existingEntry))
            {
                if (entry.Fitness > existingEntry.Fitness)
                {
                    _archive[key] = entry;
                    return true;
                }

                return false;
            }

            _archive.Add(key, entry);
            return true;
        }

        internal TIndividual SampleRandom()
        {
            if (_archive.Count == 0)
            {
                throw new InvalidOperationException();
            }

            return _archive.Values.GetRandomElement().Individual;
        }
    }
}
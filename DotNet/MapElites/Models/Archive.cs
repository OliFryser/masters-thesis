using System;
using System.Collections.Generic;
using System.Linq;
using WFC.Extensions;

namespace MapElites.Models
{
    public class Archive<TKey, TEntry, TIndividual, TBehavior>
        where TKey : IEquatable<TKey>
        where TEntry : Entry<TIndividual, TBehavior>
    {
        private readonly Dictionary<TKey, TEntry> _archive = new Dictionary<TKey, TEntry>();
        public int Count => _archive.Count;

        internal bool TryAdd(TKey key, TEntry entry)
        {
            if (_archive.TryGetValue(key, out TEntry existingEntry))
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

        internal float GetMaxFitness() => _archive.Values.Select(e => e.Fitness).Max();

        public TIndividual GetMaxFitnessIndividual()
            => _archive.Values
                .OrderByDescending(entry => entry.Fitness)
                .Select(entry => entry.Individual)
                .FirstOrDefault();
    }
}
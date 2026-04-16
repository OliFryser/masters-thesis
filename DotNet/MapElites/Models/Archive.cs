using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using MapElites.Extensions;
using WFC.Extensions;
using System.Text.Json.Serialization;

namespace MapElites.Models
{
    public class Archive<TKey, TEntry, TIndividual, TBehavior>
        where TKey : BaseKey<TKey>
        where TEntry : Entry<TIndividual, TBehavior>
    {
        private readonly Dictionary<TKey, TEntry> _archive = new Dictionary<TKey, TEntry>();
        public int Count => _archive.Count;
        public int BucketCapacity { get; }

        public Archive(int bucketCapacity)
        {
            BucketCapacity = bucketCapacity;
        }

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

        public bool TryGet(TKey key, out TEntry entry)
        {
            return _archive.TryGetValue(key, out entry);
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
            => _archive.Values.MaxBy(entry => entry.Fitness).Individual;

        public IEnumerable<TKey> Keys => _archive.Keys;

        internal float GetAverageFitness()
            => _archive.Values.Average(x => x.Fitness);
    }
}
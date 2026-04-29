using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Extensions;
using Newtonsoft.Json;

namespace MapElites.Models
{
    public class Archive<TKey, TEntry, TIndividual, TBehavior> 
        : IArchive<TKey, TEntry, TIndividual, TBehavior>, IArchiveStatisticsProvider
        where TKey : BaseKey<TKey>
        where TEntry : Entry<TIndividual, TBehavior>
    {
        [JsonProperty]
        private readonly Dictionary<TKey, TEntry> _archive = new Dictionary<TKey, TEntry>();
        public int Count => _archive.Count;
        public int BucketCapacity { get; }

        internal Archive(int bucketCapacity)
        {
            BucketCapacity = bucketCapacity;
        }
        
        [JsonConstructor]
        internal Archive(int bucketCapacity, Dictionary<TKey, TEntry> entries) : this(bucketCapacity)
        {
            _archive = entries;
        }

        public bool TryAdd(TKey key, TEntry entry)
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

        public TIndividual Sample()
        {
            if (_archive.Count == 0)
            {
                throw new InvalidOperationException();
            }

            return _archive.Values.GetRandomElement().Individual;
        }

        public float GetMaxFitness() => _archive.Values.Select(e => e.Fitness).Max();

        public IEnumerable<TKey> GetKeys() => _archive.Keys;

        public float GetAverageFitness()
            => _archive.Values.Average(x => x.Fitness);

        public float GetFeasiblePopulationPercentage()
        {
            return 1f;
        }

        public Dictionary<TKey, TEntry> GetArchiveAsDictionary()
            => _archive.ToDictionary(x => x.Key, x => x.Value);
    }
}
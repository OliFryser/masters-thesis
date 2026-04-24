using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Domain.Extensions;
using Newtonsoft.Json;

namespace MapElites.Models
{
    public class ConstrainedArchive<TKey, TEntry, TIndividual, TBehavior>
        : IArchive<TKey, TEntry, TIndividual, TBehavior>, IArchiveStatisticsProvider
        where TKey : BaseKey<TKey>
        where TEntry : ConstrainedEntry<TIndividual, TBehavior>
    {
        public class Entries
        {
            [JsonProperty]
            public TEntry? Feasible { get; private set; }
            [JsonProperty]
            public TEntry? Infeasible { get; private set; }
            
            [JsonConstructor]
            [Obsolete("For serialization purposes only")]
            public Entries()
            {
            }
            
            public Entries(TEntry entry)
            {
                if (entry.IsFeasible)
                {
                    Feasible = entry;
                }
                else
                {
                    Infeasible = entry;
                }
            }

            public bool TrySave(TEntry entry)
            {
                if (entry.IsFeasible)
                {
                    if (Feasible == null)
                    {
                        Feasible = entry;
                        return true;
                    }

                    Feasible = MostFit(entry, Feasible);
                    return true;
                }

                if (Infeasible == null)
                {
                    Infeasible = entry;
                    return true;
                }

                Infeasible = MostFeasible(entry, Infeasible);
                return true;
            }

            /// <param name="percentageForFeasible">Value between 0 and 1.</param>
            public TEntry Sample(float percentageForFeasible = 0.5f)
            {
                if (Feasible == null && Infeasible == null)
                {
                    throw new InvalidOperationException("Entries has no entries.");
                }

                if (Infeasible == null)
                {
                    return Feasible!;
                }

                if (Feasible == null)
                {
                    return Infeasible;
                }

                Random random = new Random();

                return random.NextDouble() < percentageForFeasible ? Feasible : Infeasible;
            }

            private static TEntry MostFit(TEntry first, TEntry second) =>
                first.Fitness > second.Fitness ? first : second;

            private static TEntry MostFeasible(TEntry first, TEntry second) =>
                first.Feasibility > second.Feasibility ? first : second;
        }

        public int Count => _archive.Count(kvp => kvp.Value.Feasible != null);
        
        [JsonProperty]
        public int BucketCapacity { get; }

        [JsonProperty]
        private readonly Dictionary<TKey, Entries> _archive = new Dictionary<TKey, Entries>();

        public ConstrainedArchive(int bucketCapacity)
        {
            BucketCapacity = bucketCapacity;
        }
        
        [JsonConstructor]
        public ConstrainedArchive(int bucketCapacity, Dictionary<TKey, Entries> archive) : this(bucketCapacity)
        {
            _archive = archive;
        }

        public bool TryGet(TKey key, [MaybeNullWhen(false)] out TEntry entry)
        {
            if (_archive.TryGetValue(key, out Entries? entries))
            {
                if (entries.Feasible != null)
                {
                    entry = entries.Feasible;
                    return true;
                }

                if (entries.Infeasible != null)
                {
                    entry = entries.Infeasible;
                    return true;
                }
            }

            entry = null;
            return false;
        }

        public bool TryAdd(TKey key, TEntry entry)
        {
            if (_archive.TryGetValue(key, out Entries? existing))
            {
                return existing.TrySave(entry);
            }

            Entries entries = new Entries(entry);

            _archive[key] = entries;

            return true;
        }

        public TIndividual Sample()
        {
            if (_archive.Count == 0) 
                throw new InvalidOperationException("Archive has no entries.");

            Entries? entries = _archive.GetRandomElement().Value;

            return entries.Sample().Individual;
        }

        public IEnumerable<TKey> GetKeys()
        {
            return _archive.Keys;
        }

        public Dictionary<TKey, Entries> GetArchiveAsDictionary()
        {
            return _archive.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public float GetMaxFitness()
        {
            if (_archive.Count == 0) return 0;

            if (_archive.Values.All(e => e.Feasible == null)) return 0;

            return _archive.Values.Select(e => e.Feasible?.Fitness ?? 0).Max();
        }

        public float GetAverageFitness()
        {
            if (_archive.Count == 0) return 0;

            if (_archive.Values.All(e => e.Feasible == null)) return 0;

            return _archive.Values
                .Where(e => e.Feasible != null)
                .Select(e => e.Feasible!.Fitness)
                .Average();
        }
    }
}
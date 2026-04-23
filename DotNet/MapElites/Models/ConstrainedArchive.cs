using System.Collections.Generic;

namespace MapElites.Models
{
    public class ConstrainedArchive<TKey, TEntry, TIndividual, TBehavior> 
        : IArchive<TKey, TEntry, TIndividual, TBehavior>, IArchiveStatisticsProvider
        where TKey : BaseKey<TKey>
        where TEntry : Entry<TIndividual, TBehavior>
    {
        public ConstrainedArchive(int bucketCapacity)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGet(TKey key, out TEntry entry)
        {
            throw new System.NotImplementedException();
        }

        public bool TryAdd(TKey key, TEntry entry)
        {
            throw new System.NotImplementedException();
        }

        public TIndividual SampleRandom()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<TKey> GetKeys()
        {
            throw new System.NotImplementedException();
        }

        Dictionary<TKey, TEntry> IArchive<TKey, TEntry, TIndividual, TBehavior>.GetKeysAndEntries()
        {
            throw new System.NotImplementedException();
        }

        public int Count { get; }
        public int BucketCapacity { get; }
        public float GetMaxFitness()
        {
            throw new System.NotImplementedException();
        }

        public float GetAverageFitness()
        {
            throw new System.NotImplementedException();
        }
    }
}
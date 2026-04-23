using System.Collections.Generic;

namespace MapElites.Models
{
    public interface IArchive<TKey, TEntry, out TIndividual, TBehavior>
        where TKey : BaseKey<TKey>
        where TEntry : Entry<TIndividual, TBehavior>
    {
        public bool TryGet(TKey key, out TEntry entry);
        public bool TryAdd(TKey key, TEntry entry);
        public TIndividual SampleRandom();
        public IEnumerable<TKey> GetKeys();
        internal Dictionary<TKey, TEntry> GetKeysAndEntries();
        public int BucketCapacity { get; }
    }
}
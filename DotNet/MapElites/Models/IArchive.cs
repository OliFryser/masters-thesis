using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MapElites.Models
{
    public interface IArchive<TKey, TEntry, out TIndividual, TBehavior>
        where TKey : BaseKey<TKey>
        where TEntry : Entry<TIndividual, TBehavior>
    {
        public bool TryGet(TKey key, [MaybeNullWhen(false)] out TEntry entry);
        public bool TryAdd(TKey key, TEntry entry);
        public TIndividual Sample();
        public IEnumerable<TKey> GetKeys();
        public int BucketCapacity { get; }
    }
}
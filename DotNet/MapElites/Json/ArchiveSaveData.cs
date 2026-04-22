using System.Collections.Generic;

namespace MapElites.Json
{
    public struct ArchiveSaveData<TKey, TEntry>
    {
        public Dictionary<TKey, TEntry> Dictionary { get; set; }
        public int BucketCapacity { get; set; }
    }
}
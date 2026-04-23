using System.Collections.Generic;
using MapElites.Models;

namespace MapElites.Json
{
    public static class ArchiveConverter
    {
        public static ArchiveSaveData<TKey, TEntry> GetSaveDataFromArchive<TKey, TEntry, TIndividual, TBehavior>(
            IArchive<TKey, TEntry, TIndividual, TBehavior> archive) 
            where TKey : BaseKey<TKey> 
            where TEntry : Entry<TIndividual, TBehavior> 
            => new ArchiveSaveData<TKey, TEntry>
                {
                    BucketCapacity = archive.BucketCapacity,
                    Dictionary = archive.GetKeysAndEntries(),
                };

        public static Archive<TKey, TEntry, TIndividual, TBehavior> GetArchiveFromSaveData<TKey, TEntry, TIndividual,
            TBehavior>(ArchiveSaveData<TKey, TEntry> saveData)
            where TKey : BaseKey<TKey>
            where TEntry : Entry<TIndividual, TBehavior>
            => new Archive<TKey, TEntry, TIndividual, TBehavior>(saveData.BucketCapacity, saveData.Dictionary);
    }
}
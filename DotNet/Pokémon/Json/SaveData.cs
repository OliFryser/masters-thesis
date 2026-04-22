using System.Collections.Generic;
using MapElites.Json;

namespace Pokémon.Json
{
    public struct SaveData
    {
        public int MapDimension { get; set; }
        public ArchiveSaveData<Key, Entry> Archive { get; set; }
    }
}
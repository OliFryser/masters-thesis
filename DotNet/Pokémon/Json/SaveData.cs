using System.Collections.Generic;
using MapElites.Json;
using MapElites.Models;

namespace Pokémon.Json
{
    public struct SaveData
    {
        public int MapDimension { get; set; }
        public Archive<Key, Entry, Individual, Behavior> Archive { get; set; }
    }
}
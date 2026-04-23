using MapElites.Models;

namespace Pokémon.Json
{
    public struct ConstrainedSaveData
    {
        public int MapDimensions { get; set; }
        public ConstrainedArchive<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior> Archive { get; set; }
    }
}
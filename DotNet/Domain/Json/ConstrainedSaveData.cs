using MapElites.Models;

namespace Domain.Json
{
    public struct ConstrainedSaveData
    {
        public int MapDimensions { get; set; }
        public int MapId { get; set; }
        public ConstrainedArchive<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior> Archive { get; set; }
    }
}
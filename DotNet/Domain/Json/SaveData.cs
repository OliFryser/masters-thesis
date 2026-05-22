using MapElites.Models;

namespace Domain.Json
{
    public struct SaveData
    {
        public int MapDimension { get; set; }
        public Archive<Key, Entry, Individual, Behavior> Archive { get; set; }
    }
}
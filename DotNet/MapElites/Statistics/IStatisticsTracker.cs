using MapElites.Models;

namespace MapElites.Statistics
{
    public interface IStatisticsTracker<TKey, TEntry, TIndividual, TBehavior> 
        where TKey : BaseKey<TKey> 
        where TEntry : Entry<TIndividual, TBehavior>
    {
        void AddPoint(Archive<TKey, TEntry, TIndividual, TBehavior> archive);

        void SaveToFile(string outputPath);
    }
}
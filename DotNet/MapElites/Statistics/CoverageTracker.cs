using System.Collections.Generic;
using MapElites.Models;

namespace MapElites.Statistics
{
    public class CoverageTracker<TEntry, TKey, TIndividual, TBehavior> 
        : IStatisticsTracker<TEntry, TKey, TIndividual, TBehavior> 
        where TEntry : BaseKey<TEntry> 
        where TKey : Entry<TIndividual, TBehavior>
    {
        private List<float> CoverageValues { get; } = new List<float>();
        
        public void AddPoint(Archive<TEntry, TKey, TIndividual, TBehavior> archive)
        {
            CoverageValues.Add(archive.Count / (float)archive.BucketCapacity);
        }

        public void SaveToFile(string outputPath)
        {
            FileWriter.WriteStatisticEntriesToFile(outputPath, "Coverage.txt", "Coverage", CoverageValues);
        }
    }
}
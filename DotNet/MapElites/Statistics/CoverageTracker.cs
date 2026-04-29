using System.Collections.Generic;
using MapElites.Models;

namespace MapElites.Statistics
{
    public class CoverageTracker : IStatisticsTracker
    {
        private List<float> CoverageValues { get; } = new List<float>();
        
        public void AddPoint(IArchiveStatisticsProvider archive)
        {
            CoverageValues.Add(archive.GetFeasiblePopulationSize() / (float)archive.BucketCapacity);
        }

        public void SaveToFile(string outputPath)
        {
            FileWriter.WriteStatisticEntriesToFile(outputPath, "Coverage.txt", "Coverage", CoverageValues);
        }
    }
}
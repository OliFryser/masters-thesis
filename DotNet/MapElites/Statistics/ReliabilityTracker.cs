using System.Collections.Generic;
using MapElites.Models;

namespace MapElites.Statistics
{
    public class ReliabilityTracker : IStatisticsTracker
    {
        private List<float> ReliabilityValues { get; } = new List<float>();

        public void AddPoint(IArchiveStatisticsProvider archive)
        {
            float reliability = archive.GetReliability();
            ReliabilityValues.Add(reliability);
        }

        public void SaveToFile(string outputPath)
        {
            FileWriter.WriteStatisticEntriesToFile(outputPath, "Reliability.txt", "Reliability", ReliabilityValues);
        }
    }
}
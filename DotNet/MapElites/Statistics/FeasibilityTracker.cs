using System.Collections.Generic;
using MapElites.Models;

namespace MapElites.Statistics
{
    public class FeasibilityTracker : IStatisticsTracker
    {
        private List<float> FeasibilityValues { get; } = new List<float>();

        public void AddPoint(IArchiveStatisticsProvider archive)
        {
            FeasibilityValues.Add(archive.GetFeasiblePopulationPercentage());
        }

        public void SaveToFile(string outputPath)
        {
            FileWriter.WriteStatisticEntriesToFile(outputPath, "Feasibility.txt", "Feasibility", FeasibilityValues);
        }
    }
}
using System.Collections.Generic;
using MapElites.Models;

namespace MapElites.Statistics
{
    public struct FeasibilityDataPoint
    {
        public float FeasiblePercentage;
        public int NumberOfEntries;
        public override string ToString() => $"{FeasiblePercentage}\t{NumberOfEntries}";
    }
    
    public class FeasibilityTracker : IStatisticsTracker
    {
        private List<FeasibilityDataPoint> FeasibilityValues { get; } = new List<FeasibilityDataPoint>();

        public void AddPoint(IArchiveStatisticsProvider archive)
        {
            FeasibilityDataPoint feasibilityDataPoint = new FeasibilityDataPoint()
            {
                FeasiblePercentage = archive.GetFeasiblePercentage(),
                NumberOfEntries = archive.Count
            };

            FeasibilityValues.Add(feasibilityDataPoint);
        }

        public void SaveToFile(string outputPath)
        {
            FileWriter.WriteStatisticEntriesToFile(outputPath, "Feasibility.txt", "Feasibility", FeasibilityValues);
        }
    }
}
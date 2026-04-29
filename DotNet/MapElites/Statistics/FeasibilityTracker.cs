using System.Collections.Generic;
using System.Linq;
using MapElites.Models;

namespace MapElites.Statistics
{
    public struct FeasibilityDataPoint
    {
        public int FeasiblePopulationSize;
        public int PopulationSize;
        public override string ToString() => $"{FeasiblePopulationSize}\t{PopulationSize}";
    }

    public class FeasibilityTracker : IStatisticsTracker
    {
        private List<FeasibilityDataPoint> FeasibilityValues { get; } = new List<FeasibilityDataPoint>();

        public void AddPoint(IArchiveStatisticsProvider archive)
        {
            FeasibilityDataPoint feasibilityDataPoint = new FeasibilityDataPoint()
            {
                FeasiblePopulationSize = archive.GetFeasiblePopulationSize(),
                PopulationSize = archive.Count
            };

            FeasibilityValues.Add(feasibilityDataPoint);
        }

        public void SaveToFile(string outputPath)
        {
            FileWriter.WriteStatisticEntriesToFile(outputPath, "Feasibility.txt", "Population Size",
                FeasibilityValues.Select(f => f.PopulationSize).ToList());
            FileWriter.WriteStatisticEntriesToFile(outputPath, "Feasibility.txt", "Feasible Population Size",
                FeasibilityValues.Select(f => f.FeasiblePopulationSize).ToList());
        }
    }
}
using System.Collections.Generic;
using MapElites.Models;

namespace MapElites.Statistics
{
    public class FitnessTracker : IStatisticsTracker
    {
        private List<float> MaxFitnessValues { get; } = new List<float>();
        private List<float> ReliabilityValues { get; } = new List<float>();

        public void AddPoint(IArchiveStatisticsProvider archive)
        {
            MaxFitnessValues.Add(archive.GetMaxFitness());
            ReliabilityValues.Add(archive.GetAverageFitness());
            
        }

        public void SaveToFile(string outputPath)
        {
            FileWriter.WriteStatisticEntriesToFile(outputPath, "Fitness.txt", "Max Fitness", MaxFitnessValues);
            FileWriter.WriteStatisticEntriesToFile(outputPath, "Fitness.txt", "Reliability", ReliabilityValues);
        }
    }
}
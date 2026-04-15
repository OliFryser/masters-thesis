using System.Collections.Generic;
using MapElites.Models;

namespace MapElites.Statistics
{
    public class FitnessTracker<TEntry, TKey, TIndividual, TBehavior> 
        : IStatisticsTracker<TEntry, TKey, TIndividual, TBehavior> 
        where TEntry : BaseKey<TEntry> 
        where TKey : Entry<TIndividual, TBehavior>
    {
        private List<float> MaxFitnessValues { get; } = new List<float>();
        private List<float> ReliabilityValues { get; } = new List<float>();

        public void AddPoint(Archive<TEntry, TKey, TIndividual, TBehavior> archive)
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
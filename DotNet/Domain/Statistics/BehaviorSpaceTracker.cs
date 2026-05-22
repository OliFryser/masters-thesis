using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Domain.Args;
using MapElites.Models;

namespace Domain.Statistics
{
    public static class BehaviorSpaceTracker
    {
        public static void SaveToFile(
            IArchive<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior> archive,
            int numberOfBucketsPerAxis,
            float feasibilityThreshold,
            KeyCeilings keyCeilings,
            string statisticsOutputPath)
        {
            IEnumerable<Key> keys = archive.GetKeys();
            List<string> entryStatistics = new List<string>();
            foreach (Key? key in keys)
            {
                if (!archive.TryGet(key, out ConstrainedEntry<Individual, Behavior>? entry))
                {
                    continue;
                }
                
                if (!entry.IsFeasible)
                {
                    entryStatistics.Add(
                        $"{key.FlowerBucket} {key.VariationBucket} {-1} {entry.Feasibility}"
                            .Replace(',', '.'));
                    continue;
                }
                
                entryStatistics.Add(
                    $"{key.FlowerBucket} {key.VariationBucket} {entry.Fitness} {-1}"
                        .Replace(',', '.'));
            }

            string filepath = Path.Combine(statisticsOutputPath, "BehaviorSpace.txt");
            using StreamWriter streamWriter = new StreamWriter(filepath);
            streamWriter.WriteLine("Number of Buckets Per Axis");
            streamWriter.WriteLine(numberOfBucketsPerAxis);
            streamWriter.WriteLine("Behavior Names");
            streamWriter.WriteLine(Behavior.BehaviorXName);
            streamWriter.WriteLine(Behavior.BehaviorYName);
            streamWriter.WriteLine("Behavior ceilings");
            streamWriter.WriteLine(keyCeilings.SpecialTileCeiling.ToString(CultureInfo.InvariantCulture));
            streamWriter.WriteLine(keyCeilings.VariationPercentageCeiling.ToString(CultureInfo.InvariantCulture));
            streamWriter.WriteLine("Feasibility Threshold");
            streamWriter.WriteLine(feasibilityThreshold.ToString(CultureInfo.InvariantCulture));
            streamWriter.WriteLine("Entries");
            foreach (string entry in entryStatistics)
            {
                streamWriter.WriteLine(entry);
            }

            streamWriter.Close();
        }
    }
}
using System.Collections.Generic;
using System.IO;
using MapElites.Models;

namespace Pokémon.Statistics
{
    public static class BehaviorSpaceTracker
    {
        public static void SaveToFile(
            IArchive<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior> archive,
            int numberOfBucketsPerAxis,
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
                    continue;
                }
                
                entryStatistics.Add(
                    $"{key.FlowerBucket} {key.TileTypesUsedBucket} {entry.Fitness}"
                        .Replace(',', '.'));
            }

            string filepath = Path.Combine(statisticsOutputPath, "BehaviorSpace.txt");
            using StreamWriter streamWriter = new StreamWriter(filepath);
            streamWriter.WriteLine("Number of Buckets Per Axis");
            streamWriter.WriteLine(numberOfBucketsPerAxis);
            streamWriter.WriteLine("Behavior Names");
            streamWriter.WriteLine("Flower Percentage");
            streamWriter.WriteLine("Different Tiles Used Percentage");
            streamWriter.WriteLine("Entries");
            foreach (string entry in entryStatistics)
            {
                streamWriter.WriteLine(entry);
            }

            streamWriter.Close();
        }
    }
}
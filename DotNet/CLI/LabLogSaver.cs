using System;
using System.IO;
using System.Text;
using MapElites.Args;
using Pokémon.Args;

namespace CLI
{
    public static class LabLogSaver
    {
        public static void SaveLog(
            string filepath,
            MapElitesArgs mapElitesArgs, 
            IndividualHandlerArgs individualHandlerArgs, 
            string tilemapPath)
        {
            using StreamWriter streamWriter = new StreamWriter(filepath);
            streamWriter.WriteLine($"TileMap: {tilemapPath}");
            streamWriter.WriteLine();
            streamWriter.WriteLine(GetLogFromIndividualHandlerArgs(individualHandlerArgs));
            streamWriter.WriteLine();
            streamWriter.WriteLine(GetLogFromMapElitesArgs(mapElitesArgs));
        }

        private static string GetLogFromMapElitesArgs(MapElitesArgs mapElitesArgs)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("-- MAP-Elites config --");
            stringBuilder.AppendLine($"InitializationIteration: {mapElitesArgs.InitializationIterations}");
            stringBuilder.AppendLine($"MutationIterations: {mapElitesArgs.MutationIterations}");

            return stringBuilder.ToString();
        }

        private static string GetLogFromIndividualHandlerArgs(IndividualHandlerArgs individualHandlerArgs)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("-- Individual Handler config --");
            stringBuilder.AppendLine($"EvaluationIterations: {individualHandlerArgs.EvaluationIterations}");
            stringBuilder.AppendLine($"MapDimensions: {individualHandlerArgs.EvaluationIterations}");
            
            return stringBuilder.ToString();
        }
    }
}
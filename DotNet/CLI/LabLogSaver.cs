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
            string tilemapName)
        {
            using StreamWriter streamWriter = new StreamWriter(filepath);
            streamWriter.WriteLine("--- Log for MAP-Elites run ---");
            streamWriter.WriteLine($"TileMap: {tilemapName}");
            streamWriter.WriteLine();
            streamWriter.WriteLine(GetLogFromIndividualHandlerArgs(individualHandlerArgs));
            streamWriter.WriteLine();
            streamWriter.WriteLine(GetLogFromMapElitesArgs(mapElitesArgs));
        }
        
        public static void SaveLog(
            string filepath,
            MapElitesArgs mapElitesArgs, 
            ConstrainedIndividualHandlerArgs constrainedIndividualHandlerArgs, 
            string tilemapName)
        {
            using StreamWriter streamWriter = new StreamWriter(filepath);
            streamWriter.WriteLine("--- Log for Constrained MAP-Elites run ---");
            streamWriter.WriteLine($"TileMap: {tilemapName}");
            streamWriter.WriteLine();
            streamWriter.WriteLine(GetLogFromConstrainedIndividualHandlerArgs(constrainedIndividualHandlerArgs));
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
            stringBuilder.AppendLine($"KeyCeilings: {individualHandlerArgs.KeyCeilings.ToString()}");
            
            return stringBuilder.ToString();
        }

        private static string GetLogFromConstrainedIndividualHandlerArgs(ConstrainedIndividualHandlerArgs args)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(GetLogFromIndividualHandlerArgs(args.IndividualHandlerArgs));
            stringBuilder.AppendLine($"Feasibility Threshold: {args.FeasibilityThreshold}");
            return stringBuilder.ToString();
        }
    }
}
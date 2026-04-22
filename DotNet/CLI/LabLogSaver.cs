using System;
using System.IO;
using MapElites.Args;
using Pokémon.Args;

namespace CLI
{
    public static class LabLogSaver
    {
        private static string NewLine => Environment.NewLine;
        
        public static void SaveLog(
            string filepath,
            MapElitesArgs mapElitesArgs, 
            IndividualHandlerArgs individualHandlerArgs, 
            string tilemapPath)
        {
            string saveString = $"TileMap: {tilemapPath}{NewLine}{NewLine}" +
                                $"{GetLogFromIndividualHandlerArgs(individualHandlerArgs)}{NewLine}{NewLine}" +
                                $"{GetLogFromMapElitesArgs(mapElitesArgs)}";
            
            File.WriteAllText(filepath, saveString);
        }

        private static string GetLogFromMapElitesArgs(MapElitesArgs mapElitesArgs)
        {
            return $"-- MAP-Elites config --{NewLine}" +
                   $"InitializationIteration: {mapElitesArgs.InitializationIterations}{NewLine}" +
                   $"MutationIterations: {mapElitesArgs.MutationIterations}{NewLine}";
        }

        private static string GetLogFromIndividualHandlerArgs(IndividualHandlerArgs individualHandlerArgs)
        {
            return $"-- Individual Handler config --{NewLine}" +
                   $"EvaluationIterations: {individualHandlerArgs.EvaluationIterations}{NewLine}" +
                   $"MapDimensions: {individualHandlerArgs.EvaluationIterations}{NewLine}";
        }
    }
}
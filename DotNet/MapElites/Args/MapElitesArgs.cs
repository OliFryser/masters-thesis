using System;

namespace MapElites.Args
{
    public struct MapElitesArgs
    {
        public MapElitesArgs(
            int initializationIterations, 
            int mutationIterations, 
            Action<string> logger, 
            string statisticsOutputPath)
        {
            InitializationIterations = initializationIterations;
            MutationIterations = mutationIterations;
            Logger = logger;
            StatisticsOutputPath = statisticsOutputPath;
        }

        public int InitializationIterations { get; }
        public int MutationIterations { get; }
        public Action<string> Logger { get; }
        public string StatisticsOutputPath { get; }
    }
}
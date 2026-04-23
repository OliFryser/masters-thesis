using System;
using System.Collections.Generic;
using MapElites.Statistics;

namespace MapElites.Args
{
    public struct MapElitesArgs
    {
        public MapElitesArgs(
            int initializationIterations, 
            int mutationIterations, 
            Action<string> logger, 
            string statisticsOutputPath,
            List<IStatisticsTracker> statisticsTrackers)
        {
            InitializationIterations = initializationIterations;
            MutationIterations = mutationIterations;
            Logger = logger;
            StatisticsOutputPath = statisticsOutputPath;
            StatisticsTrackers = statisticsTrackers;
        }

        public int InitializationIterations { get; }
        public int MutationIterations { get; }
        public Action<string> Logger { get; }
        public string StatisticsOutputPath { get; }
        public List<IStatisticsTracker> StatisticsTrackers { get; }
    }
}
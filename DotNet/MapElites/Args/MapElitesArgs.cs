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
            List<IStatisticsTracker> statisticsTrackers, 
            int convergeThreshold)
        {
            InitializationIterations = initializationIterations;
            MutationIterations = mutationIterations;
            Logger = logger;
            StatisticsOutputPath = statisticsOutputPath;
            StatisticsTrackers = statisticsTrackers;
            ConvergeThreshold = convergeThreshold;
        }

        public int InitializationIterations { get; }
        public int MutationIterations { get; }
        public int ConvergeThreshold { get; }
        public Action<string> Logger { get; }
        public string StatisticsOutputPath { get; }
        public List<IStatisticsTracker> StatisticsTrackers { get; }
    }
}
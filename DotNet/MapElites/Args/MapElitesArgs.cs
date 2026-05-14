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
            int convergeThreshold,
            int mapId)
        {
            InitializationIterations = initializationIterations;
            MutationIterations = mutationIterations;
            Logger = logger;
            StatisticsOutputPath = statisticsOutputPath;
            StatisticsTrackers = statisticsTrackers;
            ConvergeThreshold = convergeThreshold;
            MapId = mapId;
        }

        public int InitializationIterations { get; }
        public int MutationIterations { get; }
        public int ConvergeThreshold { get; }
        public int MapId { get; }
        public Action<string> Logger { get; }
        public string StatisticsOutputPath { get; }
        public List<IStatisticsTracker> StatisticsTrackers { get; }
    }
}
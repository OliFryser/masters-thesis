using System;

namespace MapElites.Args
{
    public struct MapElitesArgs
    {
        public MapElitesArgs(int initializationIterations, int mutationIterations, Action<string> logger)
        {
            InitializationIterations = initializationIterations;
            MutationIterations = mutationIterations;
            Logger = logger;
        }

        public int InitializationIterations { get; }
        public int MutationIterations { get; }
        public Action<string> Logger { get; }
    }
}
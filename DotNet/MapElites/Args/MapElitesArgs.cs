namespace MapElites.Args
{
    public struct MapElitesArgs
    {
        public MapElitesArgs(int initializationIterations, int mutationIterations)
        {
            InitializationIterations = initializationIterations;
            MutationIterations = mutationIterations;
        }

        public int InitializationIterations { get; }
        public int MutationIterations { get; }
    }
}
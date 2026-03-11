using MapElites.Extensions;
using MapElites.Models;

namespace MapElites
{
    public static class MapElites
    {
        public static Archive Run(int iterations, int initializationIterations)
        {
            Archive archive = new Archive();
            for (int i = 0; i < initializationIterations; i++)
            {
                Individual individual = GenerateRandomSolution();
                Fitness fitness = individual.Evaluate();
                Behavior behavior = individual.GetBehavior();
                archive.TrySaveInArchive(individual, fitness, behavior);
            }
            
            for (int i = 0; i < iterations - initializationIterations; i++)
            {
                Individual sampledIndividual = archive.SampleRandomSolution();
                
                Individual variedIndividual = sampledIndividual.GetRandomVariation();
                
                Fitness fitness = variedIndividual.Evaluate();
                Behavior behavior = variedIndividual.GetBehavior();
                archive.TrySaveInArchive(variedIndividual, fitness, behavior);
            }
            
            return archive;
        }

        private static Individual GenerateRandomSolution()
        {
            throw new System.NotImplementedException();
        }
    }
}
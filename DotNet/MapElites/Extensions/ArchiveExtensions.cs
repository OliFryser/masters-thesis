using System;
using MapElites.Models;

namespace MapElites.Extensions
{
    public static class ArchiveExtensions
    {
        
        public static Individual SampleRandomSolution(this Archive archive)
        {
            throw new System.NotImplementedException();
        }

        public static void TrySaveInArchive(this Archive archive, Individual individual, Fitness fitness, Behavior behavior)
        {
            // Check if cell is empty
            // If empty save
            // Else save if new fitness is greater than saved fitness 
            throw new NotImplementedException();
        }
    }
}
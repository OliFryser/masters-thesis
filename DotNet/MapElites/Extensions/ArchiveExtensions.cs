using System;
using MapElites.Models;

namespace MapElites.Extensions
{
    public static class ArchiveExtensions
    {
        
        public static TIndividual SampleRandomSolution<TIndividual>(this Archive<TIndividual> archive)
        {
            throw new System.NotImplementedException();
        }

        public static bool TrySaveInArchive<TIndividual>(this Archive<TIndividual> archive, Entry<TIndividual> entry)
        {
            // Check if cell is empty
            // If empty save
            // Else save if new fitness is greater than saved fitness 
            throw new NotImplementedException();
        }
    }
}
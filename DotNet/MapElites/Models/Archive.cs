using System;
using System.Collections.Generic;

namespace MapElites.Models
{
    public class Archive<TIndividual, TBehavior, TKey> where TKey : IEquatable<TKey>
    {
        internal bool TryAdd(TKey key, Entry<TIndividual, TBehavior> entry)
        {
            // Check if cell is empty
            // If empty save
            // Else save if new fitness is greater than saved fitness 
            throw new NotImplementedException();
        }

        internal TIndividual Sample()
        {
            throw new NotImplementedException();
        }
    }
}
using System.Collections.Generic;
using Core.Models;

namespace Domain
{
    public class Individual
    {
        public Individual(List<TileWeight> weights)
        {
            Weights = weights;
        }

        public List<TileWeight> Weights { get; }
    }
}
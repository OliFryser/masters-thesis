using System.Collections.Generic;
using Domain.Models;

namespace Pokémon
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
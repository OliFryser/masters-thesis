using System.Collections.Generic;
using Domain.Models;

namespace Pokémon
{
    public class Individual
    {
        public Individual(List<TileWeight> weights, int seed)
        {
            Weights = weights;
            Seed = seed;
        }

        public List<TileWeight> Weights { get; }
        public int Seed { get; }
    }
}
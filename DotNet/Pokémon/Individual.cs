using System.Collections.Generic;
using Domain.Models;

namespace Pokémon
{
    public class Individual
    {
        public Individual(Dictionary<TileType, int> weights, int seed)
        {
            Weights = weights;
            Seed = seed;
        }

        public Dictionary<TileType, int> Weights { get; }
        public int Seed { get; }
    }
}
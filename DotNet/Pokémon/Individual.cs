using System.Collections.Generic;
using Domain.Models;
using WFC.Models;

namespace Pokémon
{
    public class Individual
    {
        public Individual(Dictionary<TileType, int> weights)
        {
            Weights = weights;
        }

        public Dictionary<TileType, int> Weights { get; }
        public State? WfcInstance { get; set; }
    }
}
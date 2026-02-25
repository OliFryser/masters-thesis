using System;
using System.Collections.Generic;
using WFC.Args;

namespace WFC.Models
{
    public class AdjacencyRules
    {
        private Dictionary<TileType, Dictionary<Direction, HashSet<AdjacencyRule>>> Rules { get; set; } =
            new Dictionary<TileType, Dictionary<Direction, HashSet<AdjacencyRule>>>();
        
        public ICollection<AdjacencyRule> GetAdjacencyRules(Direction direction, TileType from)
        {
            throw new NotImplementedException();
        }

        public void AddRule(Direction direction, TileType from, AdjacencyRule adjacencyRule)
        {
            
        }
    }
}
using System.Collections.Generic;
using Domain.Models;

namespace Models
{
    public struct TileRules
    {
        public TileRules(Dictionary<Direction, int[]> validTileIds)
        {
            ValidTileIds = validTileIds;
        }

        public Dictionary<Direction, int[]> ValidTileIds { get; }
    }
}
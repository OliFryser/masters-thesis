using System.Collections;
using System.Collections.Generic;
using Domain.Models;

namespace Models
{
    public struct TileRules
    {
        public TileRules(Dictionary<Direction, BitArray> validTileIds)
        {
            ValidTileIds = validTileIds;
        }

        public Dictionary<Direction, BitArray> ValidTileIds { get; }
    }
}
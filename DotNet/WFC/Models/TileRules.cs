using System.Collections;
using System.Collections.Generic;
using Core.Models;

namespace WFC.Models
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
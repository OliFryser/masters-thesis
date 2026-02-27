using System.Collections.Generic;

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
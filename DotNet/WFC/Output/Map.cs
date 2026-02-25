using System.Collections.Generic;
using WFC.Args;
using WFC.Models;

namespace WFC.Output
{
    public class Map
    {
        public Map(IReadOnlyDictionary<Vector, TileType> coordinateToTile)
        {
            CoordinateToTile = coordinateToTile;
        }

        public IReadOnlyDictionary<Vector, TileType> CoordinateToTile { get; }
    }
}
using System.Collections.Generic;
using Domain.Models;
using WFC.Args;

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
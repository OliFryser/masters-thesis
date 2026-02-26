using System.Collections.Generic;
using Domain.Models;
using WFC.Args;
using Models;

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
using System.Collections.Generic;
using Domain.Models;

namespace WFC.Output
{
    public class Map
    {
        public Map(IReadOnlyDictionary<Vector, int> coordinateToTile)
        {
            CoordinateToTile = coordinateToTile;
        }

        public IReadOnlyDictionary<Vector, int> CoordinateToTile { get; }
    }
}
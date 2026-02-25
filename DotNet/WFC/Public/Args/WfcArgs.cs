using System.Collections.Generic;
using Domain;

namespace WFC.Public.Args
{
    public class WfcArgs
    {
        public WfcArgs(IReadOnlyCollection<Vector> coordinates)
        {
            Coordinates = coordinates;
        }

        public IReadOnlyCollection<Vector> Coordinates { get; }
        public IReadOnlyCollection<TileType> TileTypes { get; }
        public IReadOnlyCollection<AdjacencyRule> AdjacencyRules { get; }
    }
}
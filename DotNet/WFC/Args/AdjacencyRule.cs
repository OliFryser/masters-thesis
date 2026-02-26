using Models;

namespace WFC.Args
{
    public struct AdjacencyRule
    {
        public AdjacencyRule(TileType from, TileType to, Direction direction, float weight)
        {
            From = from;
            To = to;
            Direction = direction;
            Weight = weight;
        }

        public TileType From { get; }
        public TileType To { get; }
        public Direction Direction { get; }
        public float Weight { get; }
    }
}
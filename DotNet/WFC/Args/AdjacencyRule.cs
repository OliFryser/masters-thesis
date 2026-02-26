using Models;

namespace WFC.Args
{
    public struct AdjacencyRule
    {
        public AdjacencyRule(TileType from, TileType to, Direction direction)
        {
            From = from;
            To = to;
            Direction = direction;
        }

        public TileType From { get; }
        public TileType To { get; }
        public Direction Direction { get; }
    }
}
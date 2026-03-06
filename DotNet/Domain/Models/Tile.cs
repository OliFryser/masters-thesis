namespace Domain.Models
{
    public struct Tile
    {
        public Tile(int x, int y, string name)
        {
            Position = new Vector(x, y);
            Type = new TileType(name);
        }

        public Vector Position { get; set; }
        public TileType Type { get; set; }

    }
}
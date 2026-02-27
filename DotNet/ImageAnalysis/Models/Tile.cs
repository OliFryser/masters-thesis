using Domain.Models;

namespace ImageAnalysis.Models
{
    public struct Tile
    {
        public Tile(int x, int y, string name)
        {
            Position = new Vector(x, y);
            Name = name;
        }

        public Vector Position { get; set; }
        public string Name { get; set; }

    }
}
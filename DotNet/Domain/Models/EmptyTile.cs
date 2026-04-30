namespace Domain.Models
{
    public struct EmptyTile
    {
        public EmptyTile(int x, int y, int options, double entropy)
        {
            Position = new Vector(x, y);
            Options = options;
            Entropy = entropy;
        }

        public Vector Position { get; set; }
        public int Options { get; set; }
        public double Entropy { get; set; }
    }
}
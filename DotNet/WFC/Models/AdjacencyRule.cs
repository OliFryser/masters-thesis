namespace WFC.Models
{
    public struct AdjacencyRule
    {
        public AdjacencyRule(float weight, TileType to)
        {
            Weight = weight;
            To = to;
        }

        public float Weight { get; set; }
        public TileType To { get; set; }
    }
}
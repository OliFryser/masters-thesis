namespace Domain.Models
{
    public struct TileWeight
    {
        public TileWeight(TileType tileType, double weight)
        {
            TileType = tileType;
            Weight = weight;
        }
        
        public TileType TileType { get; }
        public double Weight { get; }
    }
}
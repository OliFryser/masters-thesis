using System.Collections;
using Domain.Models;

namespace WFC.Models
{
    public class Level
    {
        public Level(
            TileRules[] rules, 
            TileType[] tileTypes, 
            Vector[] position, 
            BitArray[] options, 
            double[] entropy,
            Neighbors[] neighborIndices, 
            bool[] collapsed, 
            int totalTileTypeCount, 
            double[] weights, 
            double[] sumOfWeights, 
            double[] sumOfWeightsLogWeights)
        {
            TotalTileTypeCount = totalTileTypeCount;
            TileTypes = tileTypes;
            Rules = rules;
            Position = position;
            Options = options;
            Entropy = entropy;
            NeighborIndices = neighborIndices;
            Collapsed = collapsed;
            Weights = weights;
            SumOfWeights = sumOfWeights;
            SumOfWeightsLogWeights = sumOfWeightsLogWeights;
        }
        
        public int TotalTileTypeCount { get; }
        
        // Index by tile
        public TileRules[] Rules { get; }
        public TileType[] TileTypes { get; }
        public double[] Weights { get; }

        // Index by cell 
        public Vector[] Position { get; }
        public BitArray[] Options { get; }
        public double[] Entropy { get; }
        public Neighbors[] NeighborIndices { get; }
        public bool[] Collapsed { get; }
        public double[] SumOfWeights { get; }
        public double[] SumOfWeightsLogWeights { get; }
    }
}
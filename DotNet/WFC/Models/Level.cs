using System.Collections;
using Domain.Models;
using Models;

namespace WFC.Models
{
    public class Level
    {
        public Level(
            TileRules[] rules, 
            TileType[] tileTypes, 
            Vector[] position, 
            BitArray[] options, 
            float[] entropy,
            Neighbors[] neighborIndices, 
            bool[] collapsed, 
            int totalTileTypeCount, 
            int[] weights, 
            int[] sumOfWeights, 
            float[] sumOfWeightsLogWeights)
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
        public int[] Weights { get; }

        // Index by cell 
        public Vector[] Position { get; }
        public BitArray[] Options { get; }
        public float[] Entropy { get; }
        public Neighbors[] NeighborIndices { get; }
        public bool[] Collapsed { get; }
        public int[] SumOfWeights { get; }
        public float[] SumOfWeightsLogWeights { get; }
    }
}
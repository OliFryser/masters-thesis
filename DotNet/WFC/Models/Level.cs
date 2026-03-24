using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Domain.Models;
using Models;

namespace WFC.Models
{
    public class Level
    {
        public Level(TileRules[] rules, TileType[] tileTypes, Vector[] position, BitArray[] options, float[] entropy,
            Neighbors[] neighborIndices, bool[] collapsed, int maxDepth, int totalTileTypeCount, int[] weights, int[] sumOfWeights, float[] sumOfWeightsLogWeights)
        {
            TotalTileTypeCount = totalTileTypeCount;
            TileTypes = tileTypes;
            Rules = rules;
            Position = position;
            Options = options;
            Entropy = entropy;
            NeighborIndices = neighborIndices;
            Collapsed = collapsed;
            MaxDepth = maxDepth;
            Weights = weights;
            SumOfWeights = sumOfWeights;
            SumOfWeightsLogWeights = sumOfWeightsLogWeights;
            
            // Always the same when constructing,
            // so it feels like it should be initialized in the constructor
            // Same story probably for sumOfWeights, SumOfWeightsLogWeights, etc.
            // It seems error-prone that every caller of the constructor has to do the same logic.
            SupportCount = new int[position.Length][][];
            for (int i = 0; i < SupportCount.Length; i++)
            {
                SupportCount[i] = new int[tileTypes.Length][];
                for (int j = 0; j < tileTypes.Length; j++)
                {
                    SupportCount[i][j] = Enumerable
                        .Repeat(
                            tileTypes.Length, 
                            Enum.GetValues(typeof(Direction)).Length)
                        .ToArray();
                }
            }
        }
        // Index as cell, tile, direction
        public int[][][] SupportCount { get; }
        public int TotalTileTypeCount { get; }
        public int MaxDepth { get; }

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
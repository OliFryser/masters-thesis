using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models;

namespace Models
{ 
    public class Level
    {
        // Index by tile
        public Level(TileRules[] rules, Vector[] position, HashSet<int>[] options, float[] entropy, Neighbors[] neighborIndices)
        {
            Rules = rules;
            Position = position;
            Options = options;
            Entropy = entropy;
            NeighborIndices = neighborIndices;
        }

        public TileRules[] Rules { get; }
           
        // Index by cell 
        public Vector[] Position { get; }
        public HashSet<int>[] Options { get; }
        public float[] Entropy { get; }
        public Neighbors[] NeighborIndices { get; }
    }

    public struct Neighbors
    {
        public Neighbors(Dictionary<Direction, int> indices)
        {
            Indices = indices;
        }

        public Dictionary<Direction, int> Indices { get; }
    }

    public struct TileRules
    {
        public TileRules(Dictionary<Direction, int[]> validTileIds)
        {
            ValidTileIds = validTileIds;
        }

        public Dictionary<Direction, int[]> ValidTileIds { get; }
    }
}
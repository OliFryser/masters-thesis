using System.Collections.Generic;
using Domain.Models;
using WFC.Args;

namespace Models
{ 
    public class Level
    {
        public Level(TileRules[] rules, TileType[] tileTypes, Vector[] position, HashSet<int>[] options, float[] entropy, Neighbors[] neighborIndices)
        {
            TileTypes = tileTypes;
            Rules = rules;
            Position = position;
            Options = options;
            Entropy = entropy;
            NeighborIndices = neighborIndices;
        }

        // Index by tile
        public TileRules[] Rules { get; }
        public TileType[] TileTypes { get; }
           
        // Index by cell 
        public Vector[] Position { get; }
        public HashSet<int>[] Options { get; }
        public float[] Entropy { get; }
        public Neighbors[] NeighborIndices { get; }
    }
}
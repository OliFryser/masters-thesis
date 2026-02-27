using System.Collections.Generic;
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
}
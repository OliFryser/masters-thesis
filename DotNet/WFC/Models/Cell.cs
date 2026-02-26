using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models;
using WFC.Args;

namespace Models
{
    public class Cell
    {
        public Cell(Vector position, HashSet<TileOption> options)
        {
            Position = position;
            Options = options;
        }

        public Vector Position { get; }
        public HashSet<TileOption> Options { get; }
        public float Entropy { get; set; }

        public void CalculateEntropy()
        {
            Entropy = -Options.Aggregate<TileOption, float>(0,
                (acc, tileOption) => acc + tileOption.Weight * MathF.Log(tileOption.Weight));
        }
    }
}
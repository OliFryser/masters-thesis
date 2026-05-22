using System.Collections.Generic;
using Core.Models;

namespace WFC.Models
{
    public struct Neighbors
    {
        public Neighbors(Dictionary<Direction, int> indices)
        {
            Indices = indices;
        }

        public Dictionary<Direction, int> Indices { get; }
    }
}
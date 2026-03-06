using System.Collections.Generic;
using Domain.Models;

namespace Models
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
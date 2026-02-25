using System.Collections.Generic;

namespace ImageAnalysis
{
    public class Adjacency
    {
        public Dictionary<string, int> UpNeighbors { get; } = new Dictionary<string, int>();
        public Dictionary<string, int> DownNeighbors { get; } = new Dictionary<string, int>();
        public Dictionary<string, int> LeftNeighbors { get; } = new Dictionary<string, int>();
        public Dictionary<string, int> RightNeighbors { get; } = new Dictionary<string, int>();
    }
}
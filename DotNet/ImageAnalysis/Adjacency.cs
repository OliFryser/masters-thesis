using System.Collections.Generic;

namespace ImageAnalysis;

public class Adjacency
{
    public Dictionary<string, int> UpNeighbors { get; } = new();
    public Dictionary<string, int> DownNeighbors { get; } = new();
    public Dictionary<string, int> LeftNeighbors { get; } = new();
    public Dictionary<string, int> RightNeighbors { get; } = new();
}
using System.Collections.Generic;
using System.Linq;
using Domain.Models;
using WFC.Args;
using Models;
using WFC.Models;

namespace WFC.Extensions
{
    public static class WfcArgsExtensions
    {
        public static Level ToLevel(this WfcArgs args)
        {
            int numberOfCells = args.Coordinates.Count;
            int numberOfTiles = args.TileTypes.Count;

            // convert tile ids to tile indices
            TileType[] tileTypes = args.TileTypes.ToArray();
            Dictionary<TileType, int> tileTypeToIndex = new Dictionary<TileType, int>();
            for (int i = 0; i < tileTypes.Length; i++)
            {
                tileTypeToIndex.Add(tileTypes[i], i);
            }

            TileRules[] rules = new TileRules[numberOfTiles];
            PopulateRules(rules, tileTypeToIndex, args.AdjacencyRules);

            Vector[] positions = args.Coordinates.ToArray();

            HashSet<int>[] options = new HashSet<int>[numberOfCells];
            for (int i = 0; i < options.Length; i++)
            {
                options[i] = new HashSet<int>(Enumerable.Range(0, numberOfTiles));
            }

            float[] entropy = Enumerable.Repeat<float>(numberOfTiles, numberOfCells).ToArray();
            
            Neighbors[] neighborIndices = CreateNeighborsArray(positions);
            
            bool[] collapsed = new bool[numberOfCells];
            
            Level level = new Level(
                rules, 
                tileTypes, 
                positions, 
                options, 
                entropy, 
                neighborIndices, 
                collapsed, 
                args.MaxPropagationDepth);

            return level;
        }

        public static State ToState(this WfcArgs args)
        {
            Level level = args.ToLevel();
            return new State(level);
        }

        private static Neighbors[] CreateNeighborsArray(Vector[] positions)
        {
            Neighbors[] neighbors = new Neighbors[positions.Length];

            for (int i = 0; i < positions.Length; i++)
            {
                Vector position = positions[i];

                Dictionary<Direction, int> neighborIndices = new Dictionary<Direction, int>();

                Vector upPosition = new Vector(position.X, position.Y + 1);
                Vector downPosition = new Vector(position.X, position.Y - 1);
                Vector rightPosition = new Vector(position.X + 1, position.Y);
                Vector leftPosition = new Vector(position.X - 1, position.Y);

                for (int j = 0; j < positions.Length; j++)
                {
                    Vector current = positions[j];
                    if (current == upPosition)
                    {
                        neighborIndices[Direction.North] = j;
                    }
                    else if (current == downPosition)
                    {
                        neighborIndices[Direction.South] = j;
                    }
                    else if (current == rightPosition)
                    {
                        neighborIndices[Direction.East] = j;
                    }
                    else if (current == leftPosition)
                    {
                        neighborIndices[Direction.West] = j;
                    }
                }
                neighbors[i] = new Neighbors(neighborIndices);
            }
            return neighbors;
        }

        private static void PopulateRules(TileRules[] rules, Dictionary<TileType, int> typeToIndex,  IReadOnlyCollection<AdjacencyRule> argsAdjacencyRules)
        {
            Dictionary<Direction, HashSet<int>>[]
                adjacencyRules = new Dictionary<Direction, HashSet<int>>[rules.Length];

            for (int i = 0; i < rules.Length; i++)
            {
                adjacencyRules[i] = new Dictionary<Direction, HashSet<int>>
                {
                    { Direction.North, new HashSet<int>() },
                    { Direction.East, new HashSet<int>() },
                    { Direction.South, new HashSet<int>() },
                    { Direction.West, new HashSet<int>() },
                };
            }

            foreach (AdjacencyRule adjacencyRule in argsAdjacencyRules)
            {
                int fromIndex = typeToIndex[adjacencyRule.From];
                int toIndex = typeToIndex[adjacencyRule.To];
                adjacencyRules[fromIndex][adjacencyRule.Direction].Add(toIndex);
            }

            for (int i = 0; i < rules.Length; i++)
            {
                var validTileIds = new Dictionary<Direction, int[]>();
                foreach ((Direction direction, HashSet<int> ids) in adjacencyRules[i])
                {
                    validTileIds.Add(direction, ids.ToArray());
                }

                rules[i] = new TileRules(validTileIds);
            }
        }
    }
}
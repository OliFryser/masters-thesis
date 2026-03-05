using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Domain.Models;
using Models;
using WFC.Args;
using WFC.Output;

namespace WFC.Extensions
{
    public static class LevelExtensions
    {
        public static bool IsComplete(this Level level)
        {
            foreach (HashSet<int> option in level.Options)
            {
                if (option.Count != 1)
                    return false;
            }
            return true;
        }
        
        public static bool IsInfeasible(this Level level)
        {
            foreach (HashSet<int> option in level.Options)
            {
                if (option.Count == 0)
                    return true;
            }
            return false;
        }

        public static Map? ToMap(this Level level)
        {
            if (!level.IsComplete()) return null;
            
            List<Tile> tiles = new List<Tile>();
            for (int i = 0; i < level.Position.Length; i++)
            {
                int tileIndex = level.Options[i].Single();
                TileType tileType = level.TileTypes[tileIndex];
                tiles.Add(new Tile(level.Position[i].X, level.Position[i].Y, tileType.Id));
            }
            
            return new Map(tiles);
        }
    }
}
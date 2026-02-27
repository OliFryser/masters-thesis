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
            foreach (var option in level.Options)
            {
                if (option.Count != 1)
                    return false;
            }
            return true;
        }
        
        public static bool IsInfeasible(this Level level)
        {
            foreach (var option in level.Options)
            {
                if (option.Count == 0)
                    return true;
            }
            return false;
        }

        public static Map? ToMap(this Level level, Dictionary<int, TileType> tileIndexToTileType)
        {
            if (!level.IsComplete()) return null;
            
            Dictionary<Vector, TileType> coordinateToTile = new Dictionary<Vector, TileType>();
            for (int i = 0; i < level.Position.Length; i++)
            {
                var tileIndex = level.Options[i].Single();
                var tileType = tileIndexToTileType[tileIndex];
                coordinateToTile.Add(level.Position[i], tileType);
            }
            
            ReadOnlyDictionary<Vector, TileType> readOnlyDictionary = new ReadOnlyDictionary<Vector, TileType>(coordinateToTile);
            
            return new Map(readOnlyDictionary);
        }
    }
}
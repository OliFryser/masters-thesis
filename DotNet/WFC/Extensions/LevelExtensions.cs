using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Domain.Models;
using Models;
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

        public static Map? ToMap(this Level level)
        {
            if (!level.IsComplete()) return null;
            
            Dictionary<Vector, int> coordinateToTile = new Dictionary<Vector, int>();
            for (int i = 0; i < level.Position.Length; i++)
            {
                coordinateToTile.Add(level.Position[i], level.Options[i].Single());
            }
            
            ReadOnlyDictionary<Vector, int> readOnlyDictionary = new ReadOnlyDictionary<Vector, int>(coordinateToTile);
            
            return new Map(readOnlyDictionary);
        }
    }
}
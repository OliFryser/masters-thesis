using System.Collections.Generic;
using Domain.Models;

namespace Pokémon
{
    public class LevelGeneration
    {
        public static IEnumerable<Vector> GetRectangleCoordinates(int width, int height)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    yield return new Vector(x, y);
                }
            }
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace TilemapAnalysis.Extensions
{
    public static class SymmetryAnalysis
    {
        public static bool AreEqual(this IEnumerable<Rgba32> leftBorder, IEnumerable<Rgba32> rightBorder)
        {
            return leftBorder
                .Zip(rightBorder, (a, b) => new { a, b })
                .All(pair => pair.b == pair.a);
        }
        
        public static (int matches, int notMatches) MatchingBorders(this IEnumerable<Image<Rgba32>> tiles)
        {
            int matches = 0;
            int notMatches = 0;

            List<TileBorders> tilesBorders = tiles.Select(t => t.ToTileBorders()).ToList();
            
            foreach (TileBorders tilesBorders1 in tilesBorders)
            {
                foreach (TileBorders tileBorders2 in tilesBorders)
                {
                    if (tilesBorders1.Left.AreEqual(tileBorders2.Right)
                        || tilesBorders1.Top.AreEqual(tileBorders2.Bottom))
                    {
                        matches++;
                    }
                    else
                    {
                        notMatches++;
                    }
                }
            }
            
            return (matches, notMatches);
        }
    }
}
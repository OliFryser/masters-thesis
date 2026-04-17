using System.Collections.Generic;
using SixLabors.ImageSharp.PixelFormats;

namespace TilemapAnalysis
{
    public struct TileBorders
    {
        public TileBorders(List<Rgba32> left, List<Rgba32> right, List<Rgba32> top, List<Rgba32> bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

        public List<Rgba32> Left { get; }
        public List<Rgba32> Right { get; }
        public List<Rgba32> Top { get; }
        public List<Rgba32> Bottom { get; }
    }
}
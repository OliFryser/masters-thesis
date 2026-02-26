using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageAnalysis.Extensions
{
    public static class ImageExtensions
    {
        public static string Hash(this Image<Rgba32> image)
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Rgba32 color = image[x, y];
                    stringBuilder.Append(color.ToHex());
                }
            }

            return stringBuilder.ToString().GetHashCode().ToString();
        }
    }
}
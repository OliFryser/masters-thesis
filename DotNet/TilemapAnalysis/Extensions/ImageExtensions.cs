using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

namespace TilemapAnalysis.Extensions
{
    public static class ImageExtensions
    {
        public static string Hash(this Image<Rgba32> image)
        {
            byte[] pixelBytes = new byte[image.Width * image.Height * 4];
            image.CopyPixelDataTo(pixelBytes);

            using SHA1 sha1 = SHA1.Create();

            byte[] hashBytes = sha1.ComputeHash(pixelBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }

        public static IEnumerable<Rgba32> GetLeft(this Image<Rgba32> image)
        {
            for (int y = 0; y < image.Height; y++)
            {
                yield return image.DangerousGetPixelRowMemory(y).Span[0];
            }
        }

        public static IEnumerable<Rgba32> GetRight(this Image<Rgba32> image)
        {
            int rightIndex = image.Width - 1;
            for (int y = 0; y < image.Height; y++)
            {
                yield return image.DangerousGetPixelRowMemory(y).Span[rightIndex];
            }
        }

        public static IEnumerable<Rgba32> GetTop(this Image<Rgba32> image)
        {
            return image.DangerousGetPixelRowMemory(0).ToArray();
        }

        public static IEnumerable<Rgba32> GetBottom(this Image<Rgba32> image)
        {
            int bottomIndex = image.Height - 1;
            return image.DangerousGetPixelRowMemory(bottomIndex).ToArray();
        }

        public static TileBorders ToTileBorders(this Image<Rgba32> image)
        {
            List<Rgba32> left = image.GetLeft().ToList();
            List<Rgba32> right = image.GetRight().ToList();
            List<Rgba32> top = image.GetTop().ToList();
            List<Rgba32> bottom = image.GetBottom().ToList();
            
            return new TileBorders(left, right, top, bottom);
        }
    }
}
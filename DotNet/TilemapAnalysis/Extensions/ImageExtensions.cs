using System;
using System.Security.Cryptography;
using SixLabors.ImageSharp;
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
    }
}
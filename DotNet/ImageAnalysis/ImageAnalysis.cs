using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Rectangle = SixLabors.ImageSharp.Rectangle;

namespace ImageAnalysis
{
    public static class ImageAnalysis
    {
        private static readonly string BaseDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}/../../../../ImageAnalysis/Resources/";
        
        public static void Run()
        {
            string[,] map = CreateIdMap();
            WriteTileIdsToCsv(map);
            WriteAdjacencyJson(map);
        }

        public static string[,] CreateIdMap()
        {
            string palletTownPath =
                $"{BaseDirectory}/Tilemaps/PalletTown.png";
            string outputFolder = $"{BaseDirectory}/Tiles/";
            using Image<Rgba32> image = Image.Load<Rgba32>(palletTownPath);

            const int tileSize = 16;

            int tilesX = image.Width / tileSize;
            int tilesY = image.Height / tileSize;

            HashSet<string> unique = new();

            string[,] tileIds = new string[tilesX, tilesY];

            for (int y = 0; y < tilesY; y++)
            {
                for (int x = 0; x < tilesX; x++)
                {
                    Rectangle rect = new(x * tileSize, y * tileSize, tileSize, tileSize);

                    using Image<Rgba32> tile = image.Clone(ctx => ctx.Crop(rect));

                    string hash = Hash(tile);
                    if (unique.Add(hash))
                    {
                        string tilePath = Path.Combine(outputFolder, $"{hash}.png");
                        tile.Save(tilePath);
                    }

                    tileIds[x, y] = hash;
                }
            }

            return tileIds;
        }

        public static void WriteAdjacencyJson(string[,] tileIds)
        {
            int tilesY = tileIds.GetLength(0);
            int tilesX = tileIds.GetLength(1);

            Dictionary<string, Adjacency> adjacencies = new();

            for (int y = 0; y < tilesY; y++)
            {
                for (int x = 0; x < tilesX; x++)
                {
                    string id = tileIds[x, y];

                    if (!adjacencies.TryGetValue(id, out var adjacency))
                    {
                        adjacency = new Adjacency();
                        adjacencies[id] = adjacency;
                    }

                    if (x > 0)
                        AddNeighbor(adjacency.LeftNeighbors, tileIds[x - 1, y]);

                    if (x < tilesX - 1)
                        AddNeighbor(adjacency.RightNeighbors, tileIds[x + 1, y]);

                    if (y > 0)
                        AddNeighbor(adjacency.UpNeighbors, tileIds[x, y - 1]);

                    if (y < tilesY - 1)
                        AddNeighbor(adjacency.DownNeighbors, tileIds[x, y + 1]);
                }
            }

            JsonSerializerOptions options = new()
            {
                WriteIndented = true
            };

            File.WriteAllText(
                $"{BaseDirectory}/Json/PalletTownAdjacencies.json",
                JsonSerializer.Serialize(adjacencies, options));
        }

        public static void WriteTileIdsToCsv(string[,] tiles)
        {
            string csvPath = $"{BaseDirectory}/CSV/PalletTown.csv";

            int rows = tiles.GetLength(0);
            int cols = tiles.GetLength(1);

            using StreamWriter writer = new StreamWriter(csvPath);
            for (int y = 0; y < rows; y++)
            {
                string[] row = new string[cols];
                for (int x = 0; x < cols; x++)
                {
                    // Escape quotes and wrap in quotes if necessary
                    string value = tiles[y, x] ?? "";
                    if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
                    {
                        value = "\"" + value.Replace("\"", "\"\"") + "\"";
                    }

                    row[x] = value;
                }

                writer.WriteLine(string.Join(",", row));
            }
        }

        private static string Hash(Image<Rgba32> imageToHash)
        {
            StringBuilder stringBuilder = new();

            for (int y = 0; y < imageToHash.Height; y++)
            {
                for (int x = 0; x < imageToHash.Width; x++)
                {
                    Rgba32 color = imageToHash[x, y];
                    stringBuilder.Append(color.ToHex());
                }
            }

            return stringBuilder.ToString().GetHashCode().ToString();
        }

        private static void AddNeighbor(Dictionary<string, int> neighbors, string neighborId)
        {
            if (!neighbors.TryAdd(neighborId, 0))
            {
                neighbors[neighborId]++;
            }
            else
            {
                neighbors[neighborId] = 1;
            }
        }
    }
}
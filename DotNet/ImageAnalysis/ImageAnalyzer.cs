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
    public class ImageAnalyzer
    {
        private string InputTilemapPath { get; }
        private string OutputDirectory { get; }

        public ImageAnalyzer(string inputTilemapPath, string outputDirectory)
        {
            InputTilemapPath = inputTilemapPath;
            OutputDirectory = outputDirectory;
        }
        
        public void Run()
        {
            string[,] map = CreateIdMap();
            WriteTileIdsToCsv(map);
            WriteAdjacencyJson(map);
        }

        private string[,] CreateIdMap()
        {
            string outputFolder = $"{OutputDirectory}/Tiles/";
            using Image<Rgba32> image = Image.Load<Rgba32>(InputTilemapPath);
            const int tileSize = 16;

            int tilesX = image.Width / tileSize;
            int tilesY = image.Height / tileSize;

            HashSet<string> unique = new HashSet<string>();

            string[,] tileIds = new string[tilesX, tilesY];

            for (int y = 0; y < tilesY; y++)
            {
                for (int x = 0; x < tilesX; x++)
                {
                    Rectangle rect = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);

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

        private void WriteAdjacencyJson(string[,] tileIds)
        {
            int tilesY = tileIds.GetLength(1);
            int tilesX = tileIds.GetLength(0);

            Dictionary<string, Adjacency> adjacencies = new Dictionary<string, Adjacency>();

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

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            File.WriteAllText(
                $"{OutputDirectory}/Json/PalletTownAdjacencies.json",
                JsonSerializer.Serialize(adjacencies, options));
        }

        private void WriteTileIdsToCsv(string[,] tiles)
        {
            string csvPath = $"{OutputDirectory}/CSV/PalletTown.csv";

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
            StringBuilder stringBuilder = new StringBuilder();

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
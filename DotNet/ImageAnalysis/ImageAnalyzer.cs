using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ImageAnalysis.Extensions;
using ImageAnalysis.Models;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Rectangle = SixLabors.ImageSharp.Rectangle;

namespace ImageAnalysis
{
    public class ImageAnalyzer
    {
        private string InputTilemapPath { get; }
        private string OutputDirectory { get; }
        private string TileSpritesDirectory => OutputDirectory + "/TileSprites";
        private string JsonDirectory => OutputDirectory + "/Json";
        
        public ImageAnalyzer(string inputTilemapPath, string outputDirectory)
        {
            InputTilemapPath = inputTilemapPath;
            OutputDirectory = outputDirectory;

            CreateDirectory(TileSpritesDirectory);
            CreateDirectory(JsonDirectory);
        }
        
        public void Analyze()
        {
            string[,] map = CreateIdMap();
            WriteTilePositionsToJson(map);
            WriteAdjacencyJson(map);
        }

        private void CreateDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private string[,] CreateIdMap()
        {
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
                    string hash = tile.Hash();
                    
                    if (unique.Add(hash))
                    {
                        string tilePath = Path.Combine(TileSpritesDirectory, $"{hash}.png");
                        tile.Save(tilePath);
                    }

                    tileIds[x, y] = hash;
                }
            }
            
            return tileIds;
        }

        private void WriteAdjacencyJson(string[,] tileIds)
        {
            int tilesX = tileIds.GetLength(0);
            int tilesY = tileIds.GetLength(1);

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
                $"{JsonDirectory}/Adjencencies.json",
                JsonSerializer.Serialize(adjacencies, options));
        }

        private void WriteTilePositionsToJson(string[,] tiles)
        {
            List<Tile> tilesWithPositions = new List<Tile>();
            
            int tilesX = tiles.GetLength(0);
            int tilesY = tiles.GetLength(1);
            
            for (int y = 0; y < tilesY; y++)
            {
                for (int x = 0; x < tilesX; x++)
                {
                    tilesWithPositions.Add(new Tile(x, y, tiles[x, y]));
                    
                }
            }
            
            string json = JsonConvert.SerializeObject(
                tilesWithPositions,
                Formatting.Indented);

            File.WriteAllText(
                $"{JsonDirectory}/TilesWithPositions.json",
                json);
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
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models;
using TilemapAnalysis.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Rectangle = SixLabors.ImageSharp.Rectangle;

namespace TilemapAnalysis
{
    public class TilemapAnalyzer : IDisposable
    {
        
        private Dictionary<TileType, int> TileTypeToCount { get; } = new Dictionary<TileType, int>();
        public List<TileWeight> Weights
            => TileTypeToCount.Select(kvp => new TileWeight(kvp.Key, kvp.Value)).ToList();
            
        public int TileCount => Tiles.Count;
        public int TileTypeCount => TileTypeToCount.Count;
        
        public List<Tile> Tiles { get; } = new List<Tile>();
        private List<Image<Rgba32>> TileSprites { get; } = new List<Image<Rgba32>>();
        
        public TilemapAnalyzer(string inputTilemapPath)
        {
            LoadTiles(inputTilemapPath);
        }

        public void WriteTileSpritesToFolder(string outputFolder)
        {
            foreach (Image<Rgba32>? sprite in TileSprites)
            {
                sprite.Save($"{outputFolder}/{sprite.Hash()}.png");
            }
        }

        public List<AdjacencyRule> GetAdjacencyRules()
        {
            HashSet<AdjacencyRule> rules = new HashSet<AdjacencyRule>();
            Dictionary<Vector, Tile> positionToTile = Tiles.ToDictionary(t => t.Position, t => t);

            foreach (Tile fromTile in Tiles)
            {
                Vector westNeighborPosition = new Vector(fromTile.Position.X - 1, fromTile.Position.Y);
                if (positionToTile.TryGetValue(westNeighborPosition, out Tile toTile))
                    rules.Add(new AdjacencyRule(fromTile.Type, toTile.Type, Direction.West));
                
                Vector eastNeighborPosition = new Vector(fromTile.Position.X + 1, fromTile.Position.Y);
                if (positionToTile.TryGetValue(eastNeighborPosition, out toTile))
                    rules.Add(new AdjacencyRule(fromTile.Type, toTile.Type, Direction.East));
                
                Vector northNeighborPosition = new Vector(fromTile.Position.X, fromTile.Position.Y + 1);
                if (positionToTile.TryGetValue(northNeighborPosition, out toTile))
                    rules.Add(new AdjacencyRule(fromTile.Type, toTile.Type, Direction.North));
                
                Vector southNeighborPosition = new Vector(fromTile.Position.X, fromTile.Position.Y - 1);
                if (positionToTile.TryGetValue(southNeighborPosition, out toTile))
                    rules.Add(new AdjacencyRule(fromTile.Type, toTile.Type, Direction.South));
            }

            return rules.ToList();
        }

        private void LoadTiles(string inputTilemap)
        {
            using Image<Rgba32> tilemap = Image.Load<Rgba32>(inputTilemap);
            const int tileSize = 16;

            int tilesX = tilemap.Width / tileSize;
            int tilesY = tilemap.Height / tileSize;

            HashSet<string> unique = new HashSet<string>();
            
            for (int y = 0; y < tilesY; y++)
            {
                for (int x = 0; x < tilesX; x++)
                {
                    Rectangle rect = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);

                    Image<Rgba32> tile = tilemap.Clone(ctx => ctx.Crop(rect));
                    string hash = tile.Hash();
                                        
                    if (unique.Add(hash))
                    {
                        TileSprites.Add(tile);
                    }

                    Tile newTile = new Tile(x, y, hash);

                    if (!TileTypeToCount.TryAdd(newTile.Type, 1))
                    {
                        TileTypeToCount[newTile.Type]++;
                    }

                    Tiles.Add(newTile);
                }
            }
        }

        public void Dispose()
        {
            TileSprites.ForEach(t => t.Dispose());
        }
    }
}
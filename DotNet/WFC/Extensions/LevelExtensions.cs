using System.Collections;
using System.Collections.Generic;
using Domain.Models;
using Models;
using WFC.Output;

namespace WFC.Extensions
{
    public static class LevelExtensions
    {
        public static bool IsCollapsed(this Level level)
        {
            for (var i = 0; i < level.Collapsed.Length; i++)
            {
                if (!level.Collapsed[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsCollapsed(this bool[] collapsed)
        {
            for (var i = 0; i < collapsed.Length; i++)
            {
                if (!collapsed[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsCollapsed(this BitArray collapsed)
        {
            return collapsed.HasAllSetBits();
        }

        public static bool IsFeasible(this Level level)
        {
            for (var i = 0; i < level.Options.Length; i++)
            {
                if (level.Collapsed[i])
                {
                    continue;
                }

                if (level.Options[i].HasAnySetBits())
                {
                    return true;
                }
            }

            return false;
        }

        public static bool CanStep(this Level level)
        {
            for (int i = 0; i < level.Collapsed.Length; i++)
            {
                if (!level.Collapsed[i] && level.Options[i].PopCount() > 1)
                {
                    return true;
                }
            }

            return false;
        }

        public static Map ToMap(this Level level)
        {
            List<Tile> tiles = new List<Tile>();
            for (int i = 0; i < level.Position.Length; i++)
            {
                if (!level.Collapsed[i])
                    continue;

                int tileIndex = level.Options[i].GetCollapsedTileIndex();
                TileType tileType = level.TileTypes[tileIndex];
                tiles.Add(new Tile(level.Position[i].X, level.Position[i].Y, tileType.Id));
            }

            return new Map(tiles);
        }

        public static List<EmptyTile> GetEmptyTiles(this Level level)
        {
            List<EmptyTile> tiles = new List<EmptyTile>();
            for (int i = 0; i < level.Position.Length; i++)
            {
                if (level.Collapsed[i])
                    continue;

                int options = level.Options[i].PopCount();
                float entropy = level.Entropy[i];
                tiles.Add(new EmptyTile(level.Position[i].X, level.Position[i].Y, options, entropy));
            }

            return tiles;
        }
    }
}
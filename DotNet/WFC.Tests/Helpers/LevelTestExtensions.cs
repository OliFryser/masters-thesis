using System.Collections;
using Domain.Models;
using WFC.Extensions;
using WFC.Models;

namespace WFC.Tests.Helpers;

public static class LevelTestExtensions
{
    extension(Level level)
    {
        /// <summary>
        /// Adds a neighbor connection.
        /// </summary>
        /// <param name="cellIndexFrom"></param>
        /// <param name="cellIndexTo"></param>
        /// <param name="neighborDirection">Direction is relative to the <see cref="cellIndexFrom"/></param>
        public void AddNeighborConnection(int cellIndexFrom, int cellIndexTo, Direction neighborDirection)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(cellIndexFrom, level.Position.Length);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(cellIndexTo, level.Position.Length);

            level.NeighborIndices[cellIndexFrom].Indices[neighborDirection] = cellIndexTo;
            level.NeighborIndices[cellIndexTo].Indices[neighborDirection.Reverse()] = cellIndexFrom;
        }

        public void AddAdjacencyRule(int tileFrom, int tileTo, Direction direction)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(tileFrom, level.TileTypes.Length);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(tileTo, level.TileTypes.Length);

            level.Rules[tileFrom].ValidTileIds[direction][tileTo] = true;
            level.Rules[tileTo].ValidTileIds[direction.Reverse()][tileFrom] = true;
        }

        public void ChooseOptionForCell(int cell, int tile)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(cell, level.Position.Length);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(tile, level.TileTypes.Length);

            level.Options[cell].SetAll(false);
            level.Options[cell][tile] = true;
            level.Collapsed[cell] = true;
        }

        public BitArray GetOptionsForCell(int cell)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(cell, level.Position.Length);
            
            return level.Options[cell];
        }
    }
}
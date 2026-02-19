using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Domain;
using WFC.Extensions;

namespace WFC
{
    public static class WaveFunctionCollapse
    {
        public static Level Generate(
            IReadOnlyCollection<Vector> coordinates, 
            HashSet<TileOption> tiles,
            ReadOnlyDictionary<TileType, TileRules> rules)
        {
            List<Cell> collection = coordinates.Select(coordinate => new Cell(coordinate, tiles)).ToList();
            Level level = new Level(collection);

            while (!level.IsComplete())
            {
                Cell cellToCollapse = PickCell(level);
                Cell collapsedCell = cellToCollapse.CollapseCell();
                level = ReplaceCell(level, collapsedCell);
                level = Propagate(level, cellToCollapse);
            }

            return level;
        }

        public static Cell PickCell(Level level)
        {
            throw new NotImplementedException();
        }

        private static Level ReplaceCell(Level level, Cell collapsedCell)
        {
            throw new NotImplementedException();
        }

        private static Level Propagate(Level level, Cell collapsedCell)
        {
            throw new NotImplementedException();
        }
    }
}
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="tiles"></param>
        /// <param name="rules"></param>
        /// <returns>The returned level may not be complete. </returns>
        public static Level Generate(
            IReadOnlyCollection<Vector> coordinates,
            HashSet<TileOption> tiles,
            ReadOnlyDictionary<TileType, TileRules> rules)
        {
            List<Cell> collection = coordinates.Select(coordinate => new Cell(coordinate, tiles)).ToList();
            Level level = new Level(collection);

            while (!level.IsComplete() && !level.IsInfeasible())
            {
                Cell cellToCollapse = PickCell(level);
                Cell collapsedCell = cellToCollapse.CollapseCell();
                level.SetCell(collapsedCell);
                level.Propagate(collapsedCell);
            }

            return level;
        }

        private static Cell PickCell(Level level)
        {
            return level.Cells
                .Where(cell => !cell.IsCollapsed())
                .MinBy(cell => cell.Entropy);
        }
    }
}
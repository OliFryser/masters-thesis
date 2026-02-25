using System;
using System.Linq;
using WFC.Models;
using WFC.Output;

namespace WFC.Extensions
{
    public static class LevelExtensions
    {
        public static bool IsInfeasible(this Level level) => level.Cells.All(cell => cell.IsCollapsable());
        public static bool IsComplete(this Level level) => level.Cells.All(cell => cell.IsCollapsed());
        public static void SetCell(this Level level, Cell cell) => level.CellLookup[cell.Position] = cell;
        public static void Propagate(this Level level, Cell collapsedCell)
        {
            throw new NotImplementedException();
        }

        public static Map? ToMap(this Level level)
        {
            throw new NotImplementedException();
        }
    }
}
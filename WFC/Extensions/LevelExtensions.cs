using System.Linq;
using Domain;

namespace WFC.Extensions
{
    public static class LevelExtensions
    {
        public static bool IsComplete(this Level level) => level.Cells.All(cell => cell.IsCollapsed());
        public static void SetCell(this Level level, Cell cell) => level.CellLookup[cell.Position] = cell;
    }
}
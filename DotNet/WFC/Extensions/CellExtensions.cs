using System;
using System.Collections.Generic;
using Models;

namespace WFC.Extensions
{
    public static class CellExtensions
    {
        public static bool IsCollapsed(this Cell cell) => cell.Options.Count == 1;

        public static Cell CollapseCell(this Cell cell) =>
            new Cell(cell.Position, new HashSet<TileOption> { cell.Options.GetRandomWeightedElement() });

        public static bool IsCollapsable(this Cell cell) => cell.Options.Count >= 1;
    }
}
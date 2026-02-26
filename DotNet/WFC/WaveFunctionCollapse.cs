using System.Linq;
using WFC.Args;
using WFC.Extensions;
using Models;
using WFC.Output;

namespace WFC
{
    public static class WaveFunctionCollapse
    {
        public static Result Run(in WfcArgs args)
        {
            Level level = args.ToLevel();

            while (!level.IsComplete() && !level.IsInfeasible())
            {
                Cell cellToCollapse = PickCell(level);
                Cell collapsedCell = cellToCollapse.CollapseCell();
                level.SetCell(collapsedCell);
                level.Propagate(collapsedCell);
            }

            Status status = new Status(level.IsComplete());
            return new Result(level.ToMap(), status);
        }

        private static Cell PickCell(Level level)
        {
            return level.Cells
                .Where(cell => !cell.IsCollapsed())
                .MinBy(cell => cell.Entropy);
        }
    }
}
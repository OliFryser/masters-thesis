using System.Linq;
using Domain;

namespace WFC.Extensions
{
    public static class LevelExtensions
    {
        public static bool IsComplete(this Level level) => level.Cells.All(cell => cell.IsCollapsed());
    }
}
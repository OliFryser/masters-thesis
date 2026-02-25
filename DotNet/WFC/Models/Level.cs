using System.Collections.Generic;
using System.Linq;

namespace WFC.Models
{
    public class Level
    {
        public Level(ICollection<Cell> cells)
        {
            Dictionary<Vector, Cell> dictionary = cells.ToDictionary(cell => cell.Position);
            CellLookup = dictionary;
        }

        public IDictionary<Vector, Cell> CellLookup { get; }
        public ICollection<Cell> Cells => CellLookup.Values;
    }
}
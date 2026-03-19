using System.Collections.Generic;
using Domain.Models;
using Models;
using WFC.Extensions;
using WFC.Output;

namespace WFC.Models
{
    public class State
    {
        internal Level Level { get; }
        public bool IsCollapsed => Level.IsCollapsed();
        public Map GetMap() => Level.ToMap();
        public List<EmptyTile> EmptyTiles => Level.GetEmptyTiles();
        
        public State(Level level)
        {
            Level = level;
        }
    }
}
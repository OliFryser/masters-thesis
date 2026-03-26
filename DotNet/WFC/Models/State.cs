using System;
using System.Collections.Generic;
using Domain.Models;
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
        public Random Random { get; }
        
        public State(Level level, int? seed = null)
        {
            Level = level;
            Random = seed != null ? new Random(seed.Value) : new Random();
        }
    }
}
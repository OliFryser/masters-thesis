using System;
using System.Collections.Generic;
using Core.Models;
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
        public bool CanBeStepped => !Level.IsCollapsed() && Level.IsFeasible();
        public bool HasReachedContradiction => !Level.IsCollapsed() && !Level.IsFeasible();
        public State(Level level, int? seed)
        {
            Level = level;
            Random = seed != null ? new Random(seed.Value) : new Random();
        }
    }
}
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
        
        public State(Level level)
        {
            Level = level;
        }
    }
}
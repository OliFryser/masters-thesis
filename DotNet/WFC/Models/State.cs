using Models;
using WFC.Extensions;
using WFC.Output;

namespace WFC.Models
{
    public class State
    {
        internal Level Level { get; }
        public bool IsComplete => Level.IsComplete();
        public Map GetMap() => Level.ToMap();
        
        public State(Level level)
        {
            Level = level;
        }
    }
}
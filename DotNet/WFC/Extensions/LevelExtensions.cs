using Models;

namespace WFC.Extensions
{
    public static class LevelExtensions
    {
        public static bool IsComplete(this Level level)
        {
            foreach (var option in level.Options)
            {
                if (option.Count != 1)
                    return false;
            }
            return true;
        }
        
        public static bool IsInfeasible(this Level level)
        {
            foreach (var option in level.Options)
            {
                if (option.Count == 0)
                    return true;
            }
            return false;
        }
    }
}
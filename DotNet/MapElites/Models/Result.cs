namespace MapElites.Models
{
    public struct Result<TBehavior>
    {
        public Result(float fitness, TBehavior behavior)
        {
            Fitness = fitness;
            Behavior = behavior;
        }

        public float Fitness { get; }
        public TBehavior Behavior { get; }
    }
}
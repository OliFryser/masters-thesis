namespace MapElites.Models
{
    public class Entry<TIndividual, TBehavior>
    {
        public Entry(TIndividual individual, TBehavior behavior, float fitness)
        {
            Individual = individual;
            Behavior = behavior;
            Fitness = fitness;
        }

        public TIndividual Individual { get; }
        public TBehavior Behavior { get; }
        public float Fitness { get; }
    }
}
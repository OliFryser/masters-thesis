namespace MapElites.Models
{
    public class Entry<TIndividual, TBehavior>
    {
        protected Entry(TIndividual individual, TBehavior behavior, float fitness)
        {
            Individual = individual;
            Behavior = behavior;
            Fitness = fitness;
        }

        public TIndividual Individual { get; private set; }
        public TBehavior Behavior { get; private set; }
        public float Fitness { get; private set; }
    }
}
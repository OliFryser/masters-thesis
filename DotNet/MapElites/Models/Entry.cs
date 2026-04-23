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

    public class ConstrainedEntry<TIndividual, TBehavior> : Entry<TIndividual, TBehavior>
    {
        public ConstrainedEntry(TIndividual individual, TBehavior behavior, float fitness, 
            float feasibility, float feasibilityThreshold) : base(individual, behavior, fitness)
        {
            Feasibility = feasibility;
            FeasibilityThreshold = feasibilityThreshold;
        }

        public float Feasibility { get; }
        public float FeasibilityThreshold { get; }
        public bool IsFeasible => Feasibility >= FeasibilityThreshold;
    }
}
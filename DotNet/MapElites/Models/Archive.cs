namespace MapElites.Models
{
    public class Archive<TIndividual>
    {
    }

    public class Entry<TIndividual>
    {
        public Entry(TIndividual individual, Fitness fitness, Behavior behavior)
        {
            Individual = individual;
            Fitness = fitness;
            Behavior = behavior;
        }

        public TIndividual Individual { get; }
        public Fitness Fitness { get; }
        public Behavior Behavior { get; }
    }
}
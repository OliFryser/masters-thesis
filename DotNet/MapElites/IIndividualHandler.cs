using System;
using MapElites.Models;

namespace MapElites
{
    public interface IIndividualFactory<out TIndividual>
    {
        TIndividual CreateRandom();
    }

    public interface IIndividualVariator<TIndividual>
    {
        TIndividual Mutate(TIndividual individual);
    }

    public interface IIndividualHandler<out TKey, out TEntry, TIndividual, in TBehavior>
        : IIndividualFactory<TIndividual>, IIndividualVariator<TIndividual>
        where TKey : IEquatable<TKey>
        where TEntry : Entry<TIndividual, TBehavior>
    {
        int BucketCapacity { get; }
        
        TEntry Evaluate(TIndividual individual);

        TKey GetKey(TBehavior behavior);
        
    }
}
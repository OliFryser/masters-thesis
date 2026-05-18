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

    public interface IIndividualHandler<out TKey, TEntry, TIndividual, in TBehavior>
        : IIndividualFactory<TIndividual>, IIndividualVariator<TIndividual>
        where TKey : IEquatable<TKey>
        where TEntry : Entry<TIndividual, TBehavior>
    {
        int BucketCapacity { get; }
        
        bool TryEvaluate(TIndividual individual, out TEntry entry);

        TKey GetKey(TBehavior behavior);
    }
    
    public interface IConstrainedIndividualHandler<out TKey, TEntry, TIndividual, in TBehavior>
        :  IIndividualHandler<TKey, TEntry, TIndividual, TBehavior>
        where TKey : IEquatable<TKey>
        where TEntry : ConstrainedEntry<TIndividual, TBehavior>
    {
    }

}
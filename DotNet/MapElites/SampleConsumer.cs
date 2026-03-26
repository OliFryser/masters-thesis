using System;
using MapElites.Models;

namespace MapElites
{
    public class SampleIndividual
    {
    }

    public class SampleBehavior
    {
    }

    public interface IPopulationFactory<out TIndividual>
    {
        TIndividual CreateRandom();
    }

    public interface IPopulationMutator<TIndividual>
    {
        TIndividual Mutate(TIndividual individual);
    }

    public interface IPopulationManager<out TKey, out TEntry, TIndividual, in TBehavior>
        : IPopulationFactory<TIndividual>, IPopulationMutator<TIndividual>
        where TKey : IEquatable<TKey>
        where TEntry : Entry<TIndividual, TBehavior>
    {
        TEntry Evaluate(TIndividual individual);

        TKey GetKey(TBehavior behavior);
    }

    public class SampleKey : IEquatable<SampleKey>
    {
        public bool Equals(SampleKey? other)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SampleKey)obj);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }

    public class SampleEntry : Entry<SampleIndividual, SampleBehavior>
    {
        public SampleEntry(SampleIndividual individual, SampleBehavior behavior, float fitness) : base(individual, behavior, fitness)
        {
        }
    }

    public class SamplePopulationManager : IPopulationManager<SampleKey, SampleEntry, SampleIndividual, SampleBehavior>
    {
        public SampleIndividual CreateRandom()
        {
            throw new NotImplementedException();
        }

        public SampleIndividual Mutate(SampleIndividual individual)
        {
            throw new NotImplementedException();
        }

        public SampleEntry Evaluate(SampleIndividual individual)
        {
            throw new NotImplementedException();
        }

        public SampleKey GetKey(SampleBehavior behavior)
        {
            throw new NotImplementedException();
        }
    }

    public class SampleConsumer
    {
        public void Test()
        {
            SamplePopulationManager populationManager = new SamplePopulationManager();
            Archive<SampleKey, SampleEntry, SampleIndividual, SampleBehavior> archive;
        }
    }
}
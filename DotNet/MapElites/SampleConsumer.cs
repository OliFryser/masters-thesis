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
    
    public interface IPopulationManager<TIndividual, TBehavior, out TKey> 
        : IPopulationFactory<TIndividual>, IPopulationMutator<TIndividual> where TKey : IEquatable<TKey>
    {
        Result<TBehavior> Evaluate(TIndividual individual);

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
    
    public class SamplePopulationManager : IPopulationManager<SampleIndividual, SampleBehavior, SampleKey>
    {
        public SampleIndividual CreateRandom()
        {
            throw new NotImplementedException();
        }

        public SampleIndividual Mutate(SampleIndividual individual)
        {
            throw new NotImplementedException();
        }

        public Result<SampleBehavior> Evaluate(SampleIndividual individual)
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
            Archive<SampleIndividual, SampleBehavior, SampleKey> archive = MapElites.Run(populationManager, 10, 10);
        }
    }
}
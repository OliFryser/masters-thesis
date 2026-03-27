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

    public class SampleKey : BaseKey<SampleKey>
    {
        public override bool Equals(SampleKey? other)
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
        public SampleEntry(SampleIndividual individual, SampleBehavior behavior, float fitness) : base(individual,
            behavior, fitness)
        {
        }
    }

    public class SampleIndividualHandler : IIndividualHandler<SampleKey, SampleEntry, SampleIndividual, SampleBehavior>
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
            SampleIndividualHandler individualHandler = new SampleIndividualHandler();
            Archive<SampleKey, SampleEntry, SampleIndividual, SampleBehavior> archive;
        }
    }
}
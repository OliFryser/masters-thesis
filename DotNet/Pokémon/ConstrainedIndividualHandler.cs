using System;
using System.Linq;
using MapElites;
using MapElites.Models;
using Pokémon.Args;
using WFC.Models;

namespace Pokémon
{
    public class ConstrainedIndividualHandler
        : IndividualHandler,
            IConstrainedIndividualHandler<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior>
    {
        private float FeasibilityThreshold { get; }
        private float SmoothingFactor { get; }

        public ConstrainedIndividualHandler(ConstrainedIndividualHandlerArgs args) : base(args.IndividualHandlerArgs)
        {
            FeasibilityThreshold = args.FeasibilityThreshold;
            SmoothingFactor = args.SmoothingFactor;
        }

        private static float GetFitness(Behavior[] behaviors, Behavior averageBehavior, float smoothingFactor)
        {
            float deviationSum = behaviors.Sum(behavior => behavior.GetDeviation(averageBehavior));
            float meanDeviation = deviationSum / behaviors.Length;

            return MathF.Exp(-smoothingFactor * meanDeviation);
        }

        public bool TryEvaluate(Individual individual, out ConstrainedEntry<Individual, Behavior> entry)
        {
            State[] results = SampleStates(individual);

            Behavior[] behaviors = results.Where(s => s.IsCollapsed).Select(GetBehavior).ToArray();

            if (behaviors.Length == 0)
            {
                entry = null!;
                return false;
            }
            
            Behavior averageBehavior = GetAverageBehavior(behaviors);

            float fitness = GetFitness(behaviors, averageBehavior, SmoothingFactor);

            int amountComplete = results.Count(state => state.IsCollapsed);

            float feasibility = amountComplete / (float)EvaluationIterations;

            entry = new ConstrainedEntry<Individual, Behavior>(
                individual,
                averageBehavior,
                fitness,
                feasibility,
                FeasibilityThreshold);

            return true;
        }
    }
}
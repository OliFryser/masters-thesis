using System;
using System.Linq;
using MapElites;
using MapElites.Models;
using Pokémon.Args;
using WFC;
using WFC.Args;
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


        public new ConstrainedEntry<Individual, Behavior> Evaluate(Individual individual)
        {
            State[] results = SampleStates(individual);
            
            Behavior[] behaviors = results.Select(GetBehavior).ToArray();

            Behavior averageBehavior = GetAverageBehavior(behaviors);
            
            float fitness = GetFitness(behaviors, averageBehavior, SmoothingFactor);
            
            int amountComplete = results.Count(state => state.IsCollapsed);
            
            float feasibility = amountComplete / (float)EvaluationIterations;

            ConstrainedEntry<Individual, Behavior> entry = new ConstrainedEntry<Individual, Behavior>(
                individual,
                averageBehavior,
                fitness,
                feasibility,
                FeasibilityThreshold);

            return entry;
        }

        private static float GetFitness(Behavior[] behaviors, Behavior averageBehavior, float smoothingFactor)
        {
            float deviationSum = behaviors.Sum(behavior => behavior.GetDeviation(averageBehavior));
            float meanDeviation = deviationSum / behaviors.Length;

            return MathF.Exp(-smoothingFactor * meanDeviation);
        }
    }
}